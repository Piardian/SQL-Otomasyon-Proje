namespace KutuphaneOtomasyon;

internal sealed class LoginForm : Form
{
    private readonly ComboBox cmbRole = new();
    private readonly TextBox txtUserName = new();
    private readonly TextBox txtPassword = new();
    private readonly Label lblHint = new();
    private readonly Label lblError = new();
    private Button? btnRegister;

    public LoginForm()
    {
        InitializeLoginForm();
    }

    public AppUser? LoggedInUser { get; private set; }

    private void InitializeLoginForm()
    {
        Text = "Kutuphane Otomasyon - Giris";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(430, 406);
        MinimumSize = new Size(430, 406);
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        BackColor = Color.White;

        Controls.Add(new Label
        {
            Text = "Kutuphane Girisi",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            AutoSize = true,
            Location = new Point(34, 28)
        });

        Controls.Add(new Label
        {
            Text = "Rolunu sec ve sisteme giris yap.",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(71, 85, 105),
            AutoSize = true,
            Location = new Point(38, 70)
        });

        cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRole.Font = new Font("Segoe UI", 10F);
        cmbRole.Location = new Point(40, 118);
        cmbRole.Size = new Size(350, 25);
        cmbRole.Items.AddRange(new object[] { "Admin", "Personel", "Kullanici" });
        cmbRole.SelectedIndex = 2;
        cmbRole.SelectedIndexChanged += (_, _) => UpdateHint();

        txtUserName.Font = new Font("Segoe UI", 10F);
        txtUserName.Location = new Point(40, 166);
        txtUserName.PlaceholderText = "Kullanici adi / telefon";
        txtUserName.Size = new Size(350, 25);

        txtPassword.Font = new Font("Segoe UI", 10F);
        txtPassword.Location = new Point(40, 207);
        txtPassword.PlaceholderText = "Sifre";
        txtPassword.PasswordChar = '*';
        txtPassword.Size = new Size(350, 25);
        txtPassword.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TryLogin();
            }
        };

        lblHint.Font = new Font("Segoe UI", 8.7F);
        lblHint.ForeColor = Color.FromArgb(100, 116, 139);
        lblHint.AutoSize = false;
        lblHint.Location = new Point(40, 240);
        lblHint.Size = new Size(350, 38);

        lblError.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblError.ForeColor = Color.FromArgb(220, 38, 38);
        lblError.AutoSize = false;
        lblError.Location = new Point(40, 280);
        lblError.Size = new Size(350, 22);

        var btnLogin = CreateButton("Giris Yap", Color.FromArgb(37, 99, 235), 306);
        btnLogin.Click += (_, _) => TryLogin();

        btnRegister = CreateButton("Kayit Ol", Color.FromArgb(15, 118, 110), 350);
        btnRegister.Click += (_, _) => new RegisterForm().ShowDialog(this);

        Controls.Add(cmbRole);
        Controls.Add(txtUserName);
        Controls.Add(txtPassword);
        Controls.Add(lblHint);
        Controls.Add(lblError);
        Controls.Add(btnLogin);
        Controls.Add(btnRegister);
        UpdateHint();
    }

    private static Button CreateButton(string text, Color color, int top)
    {
        var button = new Button
        {
            Text = text,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(40, top),
            Size = new Size(350, top == 306 ? 36 : 32),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void UpdateHint()
    {
        lblError.Text = string.Empty;
        lblHint.Text = SelectedRole switch
        {
            UserRole.Admin => "Admin: admin / admin123. Admin personel ekleyip silebilir.",
            UserRole.Personel => "Personel: telefon + sifre. Personel kitap ve kiralama islemlerine girer.",
            _ => "Kullanici: telefon + sifre. Giris icin personel onayi gerekir."
        };
        txtUserName.PlaceholderText = SelectedRole == UserRole.Admin ? "Kullanici adi" : "Telefon numarasi";
        if (btnRegister is not null)
        {
            btnRegister.Visible = SelectedRole == UserRole.Kullanici;
        }
    }

    private UserRole SelectedRole => cmbRole.SelectedIndex switch
    {
        1 => UserRole.Personel,
        2 => UserRole.Kullanici,
        _ => UserRole.Admin
    };

    private void TryLogin()
    {
        var user = DatabaseHelper.Login(SelectedRole, txtUserName.Text, txtPassword.Text);
        if (user is null)
        {
            lblError.Text = "Giris bilgileri hatali.";
            return;
        }

        LoggedInUser = user;
        DialogResult = DialogResult.OK;
        Close();
    }
}
