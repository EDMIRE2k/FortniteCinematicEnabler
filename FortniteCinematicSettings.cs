using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BiomeForge.FortniteCinematicSettings
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public sealed class MainForm : Form
    {
        private string _configPath;
        private readonly Label _pathLabel;
        private readonly Label _statusLabel;
        private readonly Label _readOnlyLabel;
        private readonly CheckBox _readOnlyCheck;
        private readonly ModernButton _applyButton;
        private readonly ModernButton _undoButton;
        private readonly ModernButton _refreshButton;
        private readonly ModernButton _openFolderButton;
        private readonly ModernButton _browseButton;
        private readonly TextBox _previewBox;

        public MainForm()
        {
            _configPath = FortniteSettingsService.GetDefaultConfigPath();

            Text = "Fortnite Cinematic Settings";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(760, 640);
            Size = new Size(860, 720);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(24, 24, 28);
            ForeColor = Color.White;
            DoubleBuffered = true;

            TryEnableWindows11Backdrop();

            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(28);
            root.BackColor = Color.Transparent;
            root.ColumnCount = 1;
            root.RowCount = 8;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            var title = new Label();
            title.Text = "Fortnite Cinematic Settings";
            title.AutoSize = true;
            title.Font = new Font("Segoe UI Semibold", 23F, FontStyle.Regular, GraphicsUnit.Point);
            title.Margin = new Padding(0, 0, 0, 4);
            root.Controls.Add(title, 0, 0);

            var subtitle = new Label();
            subtitle.Text = "Enables hidden cinematic graphics values in GameUserSettings.ini.";
            subtitle.AutoSize = true;
            subtitle.ForeColor = Color.FromArgb(215, 219, 230);
            subtitle.Margin = new Padding(0, 0, 0, 22);
            root.Controls.Add(subtitle, 0, 1);

            var warning = new InfoPanel(Color.FromArgb(255, 198, 92));
            warning.Margin = new Padding(0, 0, 0, 18);
            warning.SetText(
                "GPU warning",
                "These settings have immense GPU requirements. DLSS, TSR, XeSS, or another upscaler is recommended unless the system has at least an RTX 4090 or RTX 5090.");
            root.Controls.Add(warning, 0, 2);

            var card = new RoundedPanel();
            card.Padding = new Padding(18);
            card.Margin = new Padding(0, 0, 0, 18);
            card.BackColor = Color.FromArgb(38, 39, 46);
            card.Dock = DockStyle.Top;
            card.Height = 148;
            root.Controls.Add(card, 0, 3);

            var cardLayout = new TableLayoutPanel();
            cardLayout.Dock = DockStyle.Fill;
            cardLayout.ColumnCount = 2;
            cardLayout.RowCount = 4;
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            card.Controls.Add(cardLayout);

            AddMetaLabel(cardLayout, "Config", 0);
            _pathLabel = AddMetaValue(cardLayout, _configPath, 0);
            AddMetaLabel(cardLayout, "Status", 1);
            _statusLabel = AddMetaValue(cardLayout, string.Empty, 1);
            AddMetaLabel(cardLayout, "Protection", 2);
            _readOnlyLabel = AddMetaValue(cardLayout, string.Empty, 2);

            _readOnlyCheck = new CheckBox();
            _readOnlyCheck.Text = "Set GameUserSettings.ini to read-only after applying (recommended)";
            _readOnlyCheck.Checked = true;
            _readOnlyCheck.AutoSize = true;
            _readOnlyCheck.ForeColor = Color.White;
            _readOnlyCheck.Margin = new Padding(0, 2, 0, 18);
            root.Controls.Add(_readOnlyCheck, 0, 4);

            _previewBox = new TextBox();
            _previewBox.Multiline = true;
            _previewBox.ReadOnly = true;
            _previewBox.ScrollBars = ScrollBars.Vertical;
            _previewBox.BorderStyle = BorderStyle.None;
            _previewBox.BackColor = Color.FromArgb(18, 19, 23);
            _previewBox.ForeColor = Color.FromArgb(232, 236, 246);
            _previewBox.Font = new Font("Cascadia Mono", 10F, FontStyle.Regular, GraphicsUnit.Point);
            _previewBox.Dock = DockStyle.Fill;
            _previewBox.Margin = new Padding(0, 0, 0, 18);
            _previewBox.Text = FortniteSettingsService.GetPreviewText();
            root.Controls.Add(_previewBox, 0, 5);

            var buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.AutoSize = true;
            buttons.WrapContents = true;
            buttons.Margin = new Padding(0, 0, 0, 16);
            root.Controls.Add(buttons, 0, 6);

            _applyButton = new ModernButton("Apply cinematic settings", true);
            _applyButton.Click += delegate { ApplySettings(); };
            buttons.Controls.Add(_applyButton);

            _undoButton = new ModernButton("Undo from backup", false);
            _undoButton.Click += delegate { UndoSettings(); };
            buttons.Controls.Add(_undoButton);

            _refreshButton = new ModernButton("Refresh", false);
            _refreshButton.Click += delegate { RefreshState(); };
            buttons.Controls.Add(_refreshButton);

            _openFolderButton = new ModernButton("Open folder", false);
            _openFolderButton.Click += delegate { OpenFolder(); };
            buttons.Controls.Add(_openFolderButton);

            _browseButton = new ModernButton("Choose file", false);
            _browseButton.Click += delegate { ChooseFile(); };
            buttons.Controls.Add(_browseButton);

            var credits = new Label();
            credits.Text = "Credits: EDMIRE from BiomeForge";
            credits.AutoSize = true;
            credits.ForeColor = Color.FromArgb(188, 195, 210);
            credits.Margin = new Padding(0);
            root.Controls.Add(credits, 0, 7);

            RefreshState();
        }

        private static void AddMetaLabel(TableLayoutPanel panel, string text, int row)
        {
            var label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.ForeColor = Color.FromArgb(170, 178, 196);
            label.Margin = new Padding(0, 0, 18, 10);
            panel.Controls.Add(label, 0, row);
        }

        private static Label AddMetaValue(TableLayoutPanel panel, string text, int row)
        {
            var label = new Label();
            label.Text = text;
            label.AutoEllipsis = true;
            label.Dock = DockStyle.Top;
            label.ForeColor = Color.White;
            label.Margin = new Padding(0, 0, 0, 10);
            panel.Controls.Add(label, 1, row);
            return label;
        }

        private void ApplySettings()
        {
            try
            {
                if (FortniteSettingsService.IsFortniteRunning())
                {
                    var answer = MessageBox.Show(
                        this,
                        "Fortnite appears to be running. Close the game before applying so it does not overwrite GameUserSettings.ini when it exits.\r\n\r\nApply anyway?",
                        "Fortnite is running",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (answer != DialogResult.Yes)
                    {
                        return;
                    }
                }

                var result = FortniteSettingsService.Apply(_configPath, _readOnlyCheck.Checked);
                MessageBox.Show(this, result, "Cinematic settings applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Could not apply settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshState();
            }
        }

        private void UndoSettings()
        {
            try
            {
                var result = FortniteSettingsService.RestoreLatestBackup(_configPath);
                MessageBox.Show(this, result, "Backup restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Could not restore backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshState();
            }
        }

        private void OpenFolder()
        {
            var folder = Path.GetDirectoryName(_configPath);
            if (folder != null && Directory.Exists(folder))
            {
                Process.Start("explorer.exe", folder);
            }
        }

        private void ChooseFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Choose Fortnite GameUserSettings.ini";
                dialog.Filter = "GameUserSettings.ini|GameUserSettings.ini|INI files (*.ini)|*.ini|All files (*.*)|*.*";
                dialog.FileName = "GameUserSettings.ini";
                string folder = Path.GetDirectoryName(_configPath);
                if (folder != null && Directory.Exists(folder))
                {
                    dialog.InitialDirectory = folder;
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _configPath = dialog.FileName;
                    _pathLabel.Text = _configPath;
                    RefreshState();
                }
            }
        }

        private void RefreshState()
        {
            bool exists = File.Exists(_configPath);
            _statusLabel.Text = exists ? "Found" : "Not found. Launch Fortnite once, then run this tool again.";
            _statusLabel.ForeColor = exists ? Color.FromArgb(118, 232, 176) : Color.FromArgb(255, 164, 128);

            bool readOnly = exists && ((File.GetAttributes(_configPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            _readOnlyLabel.Text = exists ? (readOnly ? "Read-only is enabled" : "Read-only is off") : "Unavailable";
            _readOnlyLabel.ForeColor = readOnly ? Color.FromArgb(118, 232, 176) : Color.FromArgb(255, 198, 92);

            _applyButton.Enabled = exists;
            _undoButton.Enabled = FortniteSettingsService.HasBackup(_configPath);
            _openFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_configPath));
        }

        private void TryEnableWindows11Backdrop()
        {
            try
            {
                if (Environment.OSVersion.Version.Major < 10)
                {
                    return;
                }

                int dark = 1;
                DwmSetWindowAttribute(Handle, 20, ref dark, Marshal.SizeOf(typeof(int)));

                int rounded = 2;
                DwmSetWindowAttribute(Handle, 33, ref rounded, Marshal.SizeOf(typeof(int)));

                int mica = 2;
                DwmSetWindowAttribute(Handle, 38, ref mica, Marshal.SizeOf(typeof(int)));
            }
            catch
            {
                // Older Windows builds simply fall back to the painted dark surface.
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    }

    public static class FortniteSettingsService
    {
        private const string MainSection = "[/Script/FortniteGame.FortGameUserSettings]";
        private const string ScalabilitySection = "[ScalabilityGroups]";

        private static readonly KeyValuePair<string, string>[] MainSettings = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("bUseNanite", "True"),
            new KeyValuePair<string, string>("DesiredGlobalIlluminationQuality", "5"),
            new KeyValuePair<string, string>("DesiredReflectionQuality", "5"),
            new KeyValuePair<string, string>("PreNaniteGlobalIlluminationQuality", "5"),
            new KeyValuePair<string, string>("PreNaniteReflectionQuality", "5"),
            new KeyValuePair<string, string>("bRayTracing", "True")
        };

        private static readonly KeyValuePair<string, string>[] ScalabilitySettings = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("sg.ViewDistanceQuality", "3"),
            new KeyValuePair<string, string>("sg.AntiAliasingQuality", "3"),
            new KeyValuePair<string, string>("sg.ShadowQuality", "5"),
            new KeyValuePair<string, string>("sg.GlobalIlluminationQuality", "5"),
            new KeyValuePair<string, string>("sg.ReflectionQuality", "5"),
            new KeyValuePair<string, string>("sg.PostProcessQuality", "5"),
            new KeyValuePair<string, string>("sg.TextureQuality", "3"),
            new KeyValuePair<string, string>("sg.EffectsQuality", "3"),
            new KeyValuePair<string, string>("sg.FoliageQuality", "5"),
            new KeyValuePair<string, string>("sg.ShadingQuality", "5"),
            new KeyValuePair<string, string>("sg.LandscapeQuality", "5")
        };

        public static string GetDefaultConfigPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FortniteGame",
                "Saved",
                "Config",
                "WindowsClient",
                "GameUserSettings.ini");
        }

        public static string GetPreviewText()
        {
            var builder = new StringBuilder();
            builder.AppendLine(MainSection);
            AppendPreview(builder, MainSettings);
            builder.AppendLine();
            builder.AppendLine(ScalabilitySection);
            AppendPreview(builder, ScalabilitySettings);
            return builder.ToString();
        }

        public static string Apply(string configPath, bool setReadOnly)
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("GameUserSettings.ini was not found at the expected Fortnite config path.", configPath);
            }

            ClearReadOnly(configPath);
            string backupPath = CreateBackup(configPath);

            string original = File.ReadAllText(configPath);
            string newline = original.Contains("\r\n") ? "\r\n" : "\n";
            string updated = UpdateSection(original, newline, MainSection, MainSettings);
            updated = UpdateSection(updated, newline, ScalabilitySection, ScalabilitySettings);
            File.WriteAllText(configPath, updated, new UTF8Encoding(false));

            if (setReadOnly)
            {
                File.SetAttributes(configPath, File.GetAttributes(configPath) | FileAttributes.ReadOnly);
            }

            return "Updated GameUserSettings.ini and created backup:\r\n" + backupPath;
        }

        public static string RestoreLatestBackup(string configPath)
        {
            string backup = GetLatestBackup(configPath);
            if (backup == null)
            {
                throw new InvalidOperationException("No BiomeForge backup was found next to GameUserSettings.ini.");
            }

            string folder = Path.GetDirectoryName(configPath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (File.Exists(configPath))
            {
                ClearReadOnly(configPath);
            }

            File.Copy(backup, configPath, true);
            ClearReadOnly(configPath);
            return "Restored:\r\n" + backup;
        }

        public static bool HasBackup(string configPath)
        {
            return GetLatestBackup(configPath) != null;
        }

        public static bool IsFortniteRunning()
        {
            try
            {
                return Process.GetProcesses()
                    .Any(p => p.ProcessName.IndexOf("Fortnite", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch
            {
                return false;
            }
        }

        private static string CreateBackup(string configPath)
        {
            string folder = Path.GetDirectoryName(configPath);
            string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string backup = Path.Combine(folder, "GameUserSettings.biomeforge-backup-" + stamp + ".ini");
            File.Copy(configPath, backup, false);
            return backup;
        }

        private static string GetLatestBackup(string configPath)
        {
            string folder = Path.GetDirectoryName(configPath);
            if (folder == null || !Directory.Exists(folder))
            {
                return null;
            }

            return Directory.GetFiles(folder, "GameUserSettings.biomeforge-backup-*.ini")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
        }

        private static void ClearReadOnly(string configPath)
        {
            if (!File.Exists(configPath))
            {
                return;
            }

            FileAttributes attributes = File.GetAttributes(configPath);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(configPath, attributes & ~FileAttributes.ReadOnly);
            }
        }

        private static string UpdateSection(string content, string newline, string sectionName, KeyValuePair<string, string>[] desired)
        {
            var lines = Regex.Split(content, "\r\n|\n|\r").ToList();
            if (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            int sectionStart = FindSection(lines, sectionName);
            if (sectionStart < 0)
            {
                if (lines.Count > 0 && lines[lines.Count - 1].Length != 0)
                {
                    lines.Add(string.Empty);
                }
                lines.Add(sectionName);
                sectionStart = lines.Count - 1;
            }

            int sectionEnd = FindNextSection(lines, sectionStart + 1);
            if (sectionEnd < 0)
            {
                sectionEnd = lines.Count;
            }

            foreach (var pair in desired)
            {
                bool replaced = false;
                var keyPattern = new Regex("^\\s*" + Regex.Escape(pair.Key) + "\\s*=", RegexOptions.IgnoreCase);
                for (int i = sectionStart + 1; i < sectionEnd; i++)
                {
                    if (keyPattern.IsMatch(lines[i]))
                    {
                        lines[i] = pair.Key + "=" + pair.Value;
                        replaced = true;
                    }
                }

                if (!replaced)
                {
                    lines.Insert(sectionEnd, pair.Key + "=" + pair.Value);
                    sectionEnd++;
                }
            }

            return string.Join(newline, lines.ToArray()) + newline;
        }

        private static int FindSection(List<string> lines, string sectionName)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (string.Equals(lines[i].Trim(), sectionName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindNextSection(List<string> lines, int start)
        {
            for (int i = start; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void AppendPreview(StringBuilder builder, KeyValuePair<string, string>[] settings)
        {
            foreach (var pair in settings)
            {
                builder.Append(pair.Key);
                builder.Append('=');
                builder.AppendLine(pair.Value);
            }
        }
    }

    public sealed class ModernButton : Button
    {
        private readonly bool _primary;

        public ModernButton(string text, bool primary)
        {
            _primary = primary;
            Text = text;
            AutoSize = true;
            Height = 42;
            MinimumSize = new Size(primary ? 190 : 120, 42);
            Padding = new Padding(14, 0, 14, 0);
            Margin = new Padding(0, 0, 10, 10);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point);
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color fill = _primary ? Color.FromArgb(78, 123, 255) : Color.FromArgb(55, 57, 66);
            Color disabledFill = Color.FromArgb(48, 49, 56);
            Color textColor = Enabled ? Color.White : Color.FromArgb(140, 145, 158);

            if (!Enabled)
            {
                fill = disabledFill;
            }
            else if (ClientRectangle.Contains(PointToClient(MousePosition)))
            {
                fill = _primary ? Color.FromArgb(98, 142, 255) : Color.FromArgb(68, 70, 80);
            }

            using (var path = RoundedRect(ClientRectangle, 8))
            using (var brush = new SolidBrush(fill))
            {
                pevent.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                pevent.Graphics,
                Text,
                Font,
                ClientRectangle,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter - 1, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter - 1, bounds.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class RoundedPanel : Panel
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(ClientRectangle, 8))
            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Invalidate();
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            bounds.Width--;
            bounds.Height--;
            int diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public sealed class InfoPanel : RoundedPanel
    {
        private readonly Color _accent;
        private string _title;
        private string _body;

        public InfoPanel(Color accent)
        {
            _accent = accent;
            BackColor = Color.FromArgb(48, 44, 34);
            Height = 104;
            Padding = new Padding(18);
        }

        public void SetText(string title, string body)
        {
            _title = title;
            _body = body;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(_accent))
            {
                e.Graphics.FillRectangle(brush, 0, 0, 5, Height);
            }

            using (var titleFont = new Font("Segoe UI Semibold", 11F, FontStyle.Regular, GraphicsUnit.Point))
            using (var bodyFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point))
            using (var titleBrush = new SolidBrush(Color.White))
            using (var bodyBrush = new SolidBrush(Color.FromArgb(232, 226, 210)))
            {
                e.Graphics.DrawString(_title ?? string.Empty, titleFont, titleBrush, new RectangleF(18, 16, Width - 36, 24));
                e.Graphics.DrawString(_body ?? string.Empty, bodyFont, bodyBrush, new RectangleF(18, 44, Width - 36, 50));
            }
        }
    }
}
