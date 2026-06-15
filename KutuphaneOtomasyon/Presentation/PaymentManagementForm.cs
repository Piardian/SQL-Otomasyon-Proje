using System.Data;

namespace KutuphaneOtomasyon;

internal sealed class PaymentManagementForm : Form
{
    private readonly DataGridView grid = new();
    private readonly Label lblStatus = new();

    public PaymentManagementForm()
    {
        InitializeForm();
    }

    private void InitializeForm()
    {
        Text = "Kayıp Kitap Ödemeleri";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(940, 540);
        MinimumSize = new Size(820, 460);
        BackColor = Color.White;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 104,
            Padding = new Padding(18),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        topPanel.Controls.Add(new Label
        {
            Text = "Kayıp Kitap Ödemeleri",
            Location = new Point(18, 12),
            AutoSize = true,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        });

        topPanel.Controls.Add(new Label
        {
            Text = "Geç teslimde para alınmaz; burada sadece kayıp kitap bedeli olan bekleyen ödemeler listelenir.",
            Location = new Point(20, 44),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(71, 85, 105)
        });

        var receiveButton = CreateButton("Ödeme Alındı", Color.FromArgb(22, 163, 74), 18);
        receiveButton.Click += (_, _) => ReceiveSelectedPayment();
        var refreshButton = CreateButton("Yenile", Color.FromArgb(37, 99, 235), 154);
        refreshButton.Click += (_, _) => LoadPayments();

        topPanel.Controls.Add(receiveButton);
        topPanel.Controls.Add(refreshButton);

        lblStatus.Dock = DockStyle.Bottom;
        lblStatus.Height = 30;
        lblStatus.Padding = new Padding(18, 0, 18, 8);
        lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(71, 85, 105);

        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);

        Controls.Add(grid);
        Controls.Add(lblStatus);
        Controls.Add(topPanel);
        Load += (_, _) => LoadPayments();
    }

    private static Button CreateButton(string text, Color color, int left)
    {
        var button = new Button
        {
            Text = text,
            Location = new Point(left, 68),
            Size = new Size(122, 30),
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void LoadPayments()
    {
        grid.DataSource = DatabaseHelper.GetPendingLostBookPayments();
        HideColumn("Ödeme ID");
        HideColumn("Kira ID");
        lblStatus.Text = grid.Rows.Count == 0
            ? "Bekleyen kayıp kitap ödemesi yok."
            : $"Bekleyen kayıp kitap ödemesi: {grid.Rows.Count}";
    }

    private void HideColumn(string columnName)
    {
        var column = grid.Columns[columnName];
        if (column is not null)
        {
            column.Visible = false;
        }
    }

    private void ReceiveSelectedPayment()
    {
        if (grid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            MessageBox.Show("Ödeme almak için bir kayıt seçin.", "Ödemeler");
            return;
        }

        var user = Convert.ToString(row["Kullanıcı"]);
        var book = Convert.ToString(row["Kitap Adı"]);
        var amount = Convert.ToDecimal(row["Ödeme Tutarı"]);
        var confirm = MessageBox.Show(
            $"{user} kullanıcısından {book} için {amount:0.00} TL ödeme alındı olarak işaretlensin mi?",
            "Ödeme Alındı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            DatabaseHelper.ReceiveLostBookPayment(Convert.ToInt32(row["Ödeme ID"]));
            LoadPayments();
            lblStatus.Text = "Ödeme alındı; kayıp kitap ödeme durumu Ödendi olarak güncellendi.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ödemeler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
