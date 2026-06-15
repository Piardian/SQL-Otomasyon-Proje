namespace KutuphaneOtomasyon;

internal sealed class UserProfileForm : Form
{
    private readonly int kullaniciId;

    public UserProfileForm(int kullaniciId)
    {
        this.kullaniciId = kullaniciId;
        InitializeForm();
    }

    private void InitializeForm()
    {
        Text = "Kullanıcı Profil Detayı";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1040, 740);
        MinimumSize = new Size(940, 660);
        BackColor = Color.White;

        Load += (_, _) => LoadProfile();
    }

    private void LoadProfile()
    {
        var profile = DatabaseHelper.GetUserProfile(kullaniciId);
        Controls.Clear();

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 178,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(20)
        };

        header.Controls.Add(new Label
        {
            Text = $"{profile.Ad} {profile.Soyad}",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(20, 14),
            AutoSize = true
        });

        header.Controls.Add(CreateSectionLabel("GENEL BİLGİLER", 22, 56));
        header.Controls.Add(CreateInfoLabel($"Ad: {profile.Ad}    Soyad: {profile.Soyad}    Telefon: {profile.Telefon}    Kayıt Tarihi: {profile.KayitTarihi:dd.MM.yyyy}    Mevcut Puan: {profile.MevcutPuan}", 22, 78, Color.FromArgb(51, 65, 85)));

        header.Controls.Add(CreateSectionLabel("PUAN ÖZETİ", 22, 110));
        header.Controls.Add(CreateInfoLabel($"Toplam Kazanılan Puan: {profile.TotalEarnedPoints}    Toplam Ceza Puanı: {profile.TotalPenaltyPoints}    Mevcut Puan: {profile.MevcutPuan}", 22, 132, Color.FromArgb(15, 118, 110), true));
        header.Controls.Add(CreateInfoLabel($"Toplam Kiralama: {profile.TotalRentCount + profile.ActiveRentCount}    Aktif Kiralama: {profile.ActiveRentCount}    Geç Teslim: {profile.LateReturnCount}    Kayıp Kitap: {profile.LostBookCount}    Kayıp Borcu: {profile.TotalLostBookDebt:0.00} TL", 22, 154, Color.FromArgb(71, 85, 105), true));

        var tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5F) };
        tabs.TabPages.Add(CreateTab($"Aktif Kiralamalar ({profile.ActiveRentCount})", profile.ActiveRentals));
        tabs.TabPages.Add(CreateTab($"Geçmiş Kiralamalar ({profile.TotalRentCount})", profile.PastRentals));
        tabs.TabPages.Add(CreateTab("Gecikmiş Teslimatlar", profile.OverdueRentals));
        tabs.TabPages.Add(CreateTab("Puan Hareketleri", profile.PointMovements));
        tabs.TabPages.Add(CreateTab("Kayıp Kitaplar", profile.LostBooks));

        Controls.Add(tabs);
        Controls.Add(header);
    }

    private static Label CreateSectionLabel(string text, int left, int top)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(left, top),
            AutoSize = true
        };
    }

    private static Label CreateInfoLabel(string text, int left, int top, Color color, bool bold = false)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10F, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = color,
            Location = new Point(left, top),
            AutoSize = true
        };
    }

    private static TabPage CreateTab(string title, System.Data.DataTable table)
    {
        var page = new TabPage(title);
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            DataSource = table,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        page.Controls.Add(grid);
        return page;
    }
}

