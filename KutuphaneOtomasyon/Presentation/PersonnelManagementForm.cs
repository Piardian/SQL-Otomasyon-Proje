using System.Data;

namespace KutuphaneOtomasyon;

internal sealed class PersonnelManagementForm : Form
{
    private readonly TextBox txtAd = new();
    private readonly TextBox txtSoyad = new();
    private readonly TextBox txtTelefon = new();
    private readonly TextBox txtPassword = new();
    private readonly TextBox txtPasswordConfirm = new();
    private readonly DataGridView grid = new();
    private readonly Label lblStatus = new();

    public PersonnelManagementForm()
    {
        InitializePersonnelForm();
    }

    private void InitializePersonnelForm()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 160,
            Padding = new Padding(18),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        AddInput(topPanel, "Ad", txtAd, 18, 22, 150);
        AddInput(topPanel, "Soyad", txtSoyad, 184, 22, 150);
        AddInput(topPanel, "Telefon", txtTelefon, 350, 22, 180);
        AddInput(topPanel, "Sifre", txtPassword, 546, 22, 150);
        txtPassword.PasswordChar = '*';
        AddInput(topPanel, "Sifre Dogrulama", txtPasswordConfirm, 712, 22, 170);
        txtPasswordConfirm.PasswordChar = '*';

        var btnAdd = CreateButton("Personel Ekle", Color.FromArgb(22, 163, 74), 952, 42);
        btnAdd.Click += (_, _) => AddPersonel();
        topPanel.Controls.Add(btnAdd);

        var btnDelete = CreateButton("Secili Personeli Sil", Color.FromArgb(220, 38, 38), 952, 78);
        btnDelete.Click += (_, _) => DeleteSelectedPersonel();
        topPanel.Controls.Add(btnDelete);

        lblStatus.Location = new Point(18, 104);
        lblStatus.Size = new Size(700, 22);
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
        topPanel.Controls.Add(lblStatus);

        grid.Dock = DockStyle.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;

        Controls.Add(grid);
        Controls.Add(topPanel);

        Load += (_, _) =>
        {
            LoadPersoneller();
        };
    }

    private static void AddInput(Panel panel, string label, TextBox textBox, int left, int top, int width)
    {
        panel.Controls.Add(CreateLabel(label, left, top));
        textBox.Location = new Point(left, top + 23);
        textBox.Size = new Size(width, 25);
        textBox.Font = new Font("Segoe UI", 9.5F);
        panel.Controls.Add(textBox);
    }

    private static Label CreateLabel(string text, int left, int top)
    {
        return new Label
        {
            Text = text,
            Location = new Point(left, top),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85)
        };
    }

    private static Button CreateButton(string text, Color color, int left, int top)
    {
        var button = new Button
        {
            Text = text,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(left, top),
            Size = new Size(170, 30),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void LoadPersoneller()
    {
        grid.DataSource = DatabaseHelper.GetPersoneller();
        var personelIdColumn = grid.Columns["PersonelID"];
        if (personelIdColumn is not null)
        {
            personelIdColumn.Visible = false;
        }

        var kutuphaneIdColumn = grid.Columns["KutuphaneID"];
        if (kutuphaneIdColumn is not null)
        {
            kutuphaneIdColumn.Visible = false;
        }

        lblStatus.Text = $"Toplam personel: {grid.Rows.Count}";
    }

    private void AddPersonel()
    {
        if (txtAd.Text.Trim().Length == 0 || txtSoyad.Text.Trim().Length == 0 || txtTelefon.Text.Trim().Length == 0 || txtPassword.Text.Length == 0)
        {
            MessageBox.Show("Ad, soyad, telefon ve sifre zorunludur.", "Personel Yonetimi");
            return;
        }

        if (txtPassword.Text != txtPasswordConfirm.Text)
        {
            MessageBox.Show("Sifre ve sifre dogrulama ayni olmalidir.", "Personel Yonetimi");
            return;
        }

        try
        {
            var id = DatabaseHelper.AddPersonel(txtAd.Text, txtSoyad.Text, txtTelefon.Text, txtPassword.Text);
            txtAd.Clear();
            txtSoyad.Clear();
            txtTelefon.Clear();
            txtPassword.Clear();
            txtPasswordConfirm.Clear();
            LoadPersoneller();
            lblStatus.Text = $"Personel eklendi. PersonelID: {id}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Personel Yonetimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteSelectedPersonel()
    {
        if (grid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            MessageBox.Show("Silmek icin personel secin.", "Personel Yonetimi");
            return;
        }

        var personelId = Convert.ToInt32(row["PersonelID"]);
        var result = MessageBox.Show("Secili personel silinsin mi?", "Personel Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            DatabaseHelper.DeletePersonel(personelId);
            LoadPersoneller();
            lblStatus.Text = "Personel silindi.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Personel Yonetimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
