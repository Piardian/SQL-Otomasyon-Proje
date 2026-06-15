using System.Data;

namespace KutuphaneOtomasyon;

internal sealed class RentalRequestsForm : Form
{
    private readonly AppUser currentUser;
    private readonly DataGridView grid = new();
    private readonly Label lblStatus = new();

    public RentalRequestsForm(AppUser currentUser)
    {
        this.currentUser = currentUser;
        InitializeRequests();
    }

    private void InitializeRequests()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        var topPanel = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = Color.FromArgb(248, 250, 252), Padding = new Padding(18) };
        var btnApprove = new Button
        {
            Text = "Secili Istegi Onayla",
            BackColor = Color.FromArgb(22, 163, 74),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(18, 14),
            Size = new Size(180, 34),
            UseVisualStyleBackColor = false
        };
        btnApprove.FlatAppearance.BorderSize = 0;
        btnApprove.Click += (_, _) => ApproveSelected();

        lblStatus.Location = new Point(220, 21);
        lblStatus.Size = new Size(700, 24);
        lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
        topPanel.Controls.Add(btnApprove);
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

        Controls.Add(grid);
        Controls.Add(topPanel);
        Load += (_, _) => LoadRequests();
    }

    private void LoadRequests()
    {
        grid.DataSource = DatabaseHelper.GetRentalRequests();
        var idColumn = grid.Columns["RezervasyonID"];
        if (idColumn is not null)
        {
            idColumn.Visible = false;
        }

        lblStatus.Text = $"Bekleyen istek: {grid.Rows.Count}";
    }

    private void ApproveSelected()
    {
        if (grid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            MessageBox.Show("Onaylamak icin istek secin.", "Kiralama Istekleri");
            return;
        }

        try
        {
            var personelId = currentUser.RecordId ?? DatabaseHelper.GetDefaultPersonelId();
            DatabaseHelper.ApproveRentalRequest(Convert.ToInt32(row["RezervasyonID"]), personelId);
            LoadRequests();
            lblStatus.Text = "Kiralama onaylandi.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kiralama Istekleri", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
