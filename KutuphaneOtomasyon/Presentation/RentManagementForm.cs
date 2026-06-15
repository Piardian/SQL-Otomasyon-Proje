using System.Data;

namespace KutuphaneOtomasyon;

internal sealed class RentManagementForm : Form
{
    private readonly AppUser currentUser;
    private readonly ComboBox cmbFilter = new();
    private readonly DataGridView grid = new();
    private readonly DataGridView requestGrid = new();
    private readonly Label lblStatus = new();
    private readonly Label lblRequestStatus = new();

    public RentManagementForm(AppUser currentUser)
    {
        this.currentUser = currentUser;
        InitializeForm();
    }

    private void InitializeForm()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 112,
            Padding = new Padding(18),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var titleLabel = new Label
        {
            Text = "Kira Yönetimi - Sadece Kiralama İşlemleri",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(18, 10),
            AutoSize = true
        };

        var helpLabel = new Label
        {
            Text = "Bu tabloda kitap müsaitliği gösterilmez; her satır bir kiralama kaydıdır.",
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(20, 34),
            AutoSize = true
        };

        cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilter.Font = new Font("Segoe UI", 9.5F);
        cmbFilter.Location = new Point(18, 62);
        cmbFilter.Size = new Size(190, 25);
        cmbFilter.Items.AddRange(new object[]
        {
            "Tüm Kiralamalar",
            "Aktif Kiralamalar",
            "Gecikmişler",
            "Teslim Edilenler",
            "Geç Teslim Edilenler",
            "Kayıp Kitaplar",
            "İptal Edilenler"
        });
        cmbFilter.SelectedIndex = 0;
        cmbFilter.SelectedIndexChanged += (_, _) => LoadRentals();

        var refreshButton = CreateButton("Yenile", Color.FromArgb(37, 99, 235), 224);
        refreshButton.Click += (_, _) => LoadRentals();

        var returnButton = CreateButton("Teslim Al", Color.FromArgb(22, 163, 74), 330);
        returnButton.Click += (_, _) => ReturnSelectedRental();

        var lostButton = CreateButton("Kayıp İşaretle", Color.FromArgb(220, 38, 38), 436, 112);
        lostButton.Click += (_, _) => MarkSelectedAsLost();

        var newRentalButton = CreateButton("Yeni Kiralama", Color.FromArgb(15, 118, 110), 558, 118);
        newRentalButton.Click += (_, _) => OpenRentalRequests();

        var detailButton = CreateButton("Detay Görüntüle", Color.FromArgb(100, 116, 139), 686, 124);
        detailButton.Click += (_, _) => ShowSelectedDetail();

        var paymentButton = CreateButton("Ödemeler", Color.FromArgb(124, 58, 237), 820, 106);
        paymentButton.Click += (_, _) => OpenPayments();

        lblStatus.Location = new Point(18, 90);
        lblStatus.Size = new Size(980, 22);
        lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(71, 85, 105);

        topPanel.Controls.Add(titleLabel);
        topPanel.Controls.Add(helpLabel);
        topPanel.Controls.Add(cmbFilter);
        topPanel.Controls.Add(refreshButton);
        topPanel.Controls.Add(returnButton);
        topPanel.Controls.Add(lostButton);
        topPanel.Controls.Add(newRentalButton);
        topPanel.Controls.Add(detailButton);
        topPanel.Controls.Add(paymentButton);
        topPanel.Controls.Add(lblStatus);

        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;

        var requestPanel = CreateRequestPanel();

        Controls.Add(grid);
        Controls.Add(requestPanel);
        Controls.Add(topPanel);
        Load += (_, _) => RefreshAll();
    }

    private static Button CreateButton(string text, Color color, int left, int width = 96)
    {
        var button = new Button
        {
            Text = text,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(left, 60),
            Size = new Size(width, 30),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void LoadRentals()
    {
        var filter = Convert.ToString(cmbFilter.SelectedItem) ?? "Tüm Kiralamalar";
        grid.DataSource = DatabaseHelper.GetRentalManagementRows(filter);
        lblStatus.Text = $"{filter}: {grid.Rows.Count} kiralama kaydı listeleniyor. Bu ekranda kitap müsaitliği değil, kiralama işleminin durumu gösterilir.";
    }

    private Panel CreateRequestPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 220,
            Padding = new Padding(18, 10, 18, 14),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 42,
            BackColor = Color.FromArgb(248, 250, 252)
        };

        header.Controls.Add(new Label
        {
            Text = "Kira İstekleri",
            Dock = DockStyle.Left,
            Width = 220,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        });

        var approveButton = new Button
        {
            Text = "Seçili İsteği Onayla",
            Dock = DockStyle.Right,
            Width = 170,
            BackColor = Color.FromArgb(22, 163, 74),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            UseVisualStyleBackColor = false
        };
        approveButton.FlatAppearance.BorderSize = 0;
        approveButton.Click += (_, _) => ApproveSelectedRequest();

        var refreshRequestsButton = new Button
        {
            Text = "İstekleri Yenile",
            Dock = DockStyle.Right,
            Width = 135,
            BackColor = Color.FromArgb(37, 99, 235),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            UseVisualStyleBackColor = false,
            Margin = new Padding(8, 0, 8, 0)
        };
        refreshRequestsButton.FlatAppearance.BorderSize = 0;
        refreshRequestsButton.Click += (_, _) => LoadRequests();

        header.Controls.Add(approveButton);
        header.Controls.Add(refreshRequestsButton);

        lblRequestStatus.Dock = DockStyle.Bottom;
        lblRequestStatus.Height = 24;
        lblRequestStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRequestStatus.ForeColor = Color.FromArgb(71, 85, 105);

        requestGrid.Dock = DockStyle.Fill;
        requestGrid.ReadOnly = true;
        requestGrid.AllowUserToAddRows = false;
        requestGrid.AllowUserToDeleteRows = false;
        requestGrid.MultiSelect = false;
        requestGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        requestGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        requestGrid.BackgroundColor = Color.White;
        requestGrid.BorderStyle = BorderStyle.FixedSingle;
        requestGrid.RowHeadersVisible = false;

        panel.Controls.Add(requestGrid);
        panel.Controls.Add(lblRequestStatus);
        panel.Controls.Add(header);
        return panel;
    }

    private void RefreshAll()
    {
        LoadRentals();
        LoadRequests();
    }

    private void LoadRequests()
    {
        requestGrid.DataSource = DatabaseHelper.GetRentalRequests();
        var idColumn = requestGrid.Columns["RezervasyonID"];
        if (idColumn is not null)
        {
            idColumn.Visible = false;
        }

        lblRequestStatus.Text = requestGrid.Rows.Count == 0
            ? "Bekleyen kira isteği yok."
            : $"Bekleyen kira isteği: {requestGrid.Rows.Count}. Kullanıcıların kitap kiralama talepleri burada onaylanır.";
    }

    private void ApproveSelectedRequest()
    {
        if (requestGrid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            MessageBox.Show("Onaylamak için bir kira isteği seçin.", "Kira İstekleri");
            return;
        }

        try
        {
            var personelId = currentUser.RecordId ?? DatabaseHelper.GetDefaultPersonelId();
            DatabaseHelper.ApproveRentalRequest(Convert.ToInt32(row["RezervasyonID"]), personelId);
            RefreshAll();
            lblRequestStatus.Text = "Kira isteği onaylandı ve aktif kiralamalara eklendi.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kira İstekleri", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReturnSelectedRental()
    {
        if (!TryGetSelectedRental(out var row))
        {
            return;
        }

        var status = Convert.ToString(row["Durum"]);
        if (status is not ("Aktif" or "Gecikmiş"))
        {
            MessageBox.Show("Sadece aktif veya gecikmiş kiralama teslim alınabilir.", "Kira Yönetimi");
            return;
        }

        try
        {
            DatabaseHelper.ReturnRental(Convert.ToInt32(row["Kira ID"]));
            RefreshAll();
            lblStatus.Text = "Teslim tarihi kaydedildi; kitap müsait oldu, puan/ceza hesaplandı.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kira Yönetimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MarkSelectedAsLost()
    {
        if (!TryGetSelectedRental(out var row))
        {
            return;
        }

        var status = Convert.ToString(row["Durum"]);
        if (status is not ("Aktif" or "Gecikmiş"))
        {
            MessageBox.Show("Sadece aktif veya gecikmiş kiralama kayıp işaretlenebilir.", "Kira Yönetimi");
            return;
        }

        var result = MessageBox.Show("Seçili kitap kayıp olarak işaretlensin mi?", "Kayıp Kitap", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            DatabaseHelper.MarkRentalAsLost(Convert.ToInt32(row["Kira ID"]));
            RefreshAll();
            lblStatus.Text = "Kira durumu Kayıp oldu; kitap stoktan düştü, ceza puanı ve ödeme tutarı yazıldı.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kira Yönetimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenRentalRequests()
    {
        using var form = new RentalRequestsForm(currentUser)
        {
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(900, 560),
            Text = "Yeni Kiralama / Kiralama İstekleri"
        };
        form.ShowDialog(this);
        RefreshAll();
    }

    private void OpenPayments()
    {
        using var form = new PaymentManagementForm();
        form.ShowDialog(this);
        RefreshAll();
    }

    private void ShowSelectedDetail()
    {
        if (!TryGetSelectedRental(out var row))
        {
            return;
        }

        var lines = row.Row.Table.Columns
            .Cast<DataColumn>()
            .Select(column => $"{column.ColumnName}: {row[column.ColumnName]}")
            .ToArray();
        MessageBox.Show(string.Join(Environment.NewLine, lines), "Kiralama Detayı");
    }

    private bool TryGetSelectedRental(out DataRowView row)
    {
        if (grid.CurrentRow?.DataBoundItem is DataRowView selected)
        {
            row = selected;
            return true;
        }

        row = null!;
        MessageBox.Show("Önce bir kiralama kaydı seçin.", "Kira Yönetimi");
        return false;
    }
}

