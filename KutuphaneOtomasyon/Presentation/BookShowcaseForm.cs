using System.Data;

namespace KutuphaneOtomasyon;

internal sealed class BookShowcaseForm : Form
{
    private readonly AppUser currentUser;
    private readonly FlowLayoutPanel bookFlow = new();
    private readonly DataGridView requestGrid = new();
    private readonly Label lblStatus = new();
    private readonly Label lblRequestStatus = new();
    private readonly TextBox txtSearch = new();
    private readonly Label lblCover = new();
    private readonly Label lblDetailTitle = new();
    private readonly Label lblDetailMeta = new();
    private readonly Label lblDetailStatus = new();
    private readonly Label lblDetailAvailability = new();
    private readonly Label lblDetailIsbn = new();
    private readonly Label lblDetailSummary = new();
    private readonly Button btnRent;
    private readonly Button btnCancel;
    private DataTable books = new();
    private DataRow? selectedBook;
    private Panel? selectedCard;
    private readonly Dictionary<int, Image?> coverCache = new();

    public BookShowcaseForm(AppUser currentUser)
    {
        this.currentUser = currentUser;
        btnRent = CreateButton("Bu Kitabı Kirala", Color.FromArgb(37, 99, 235), 0, 0, 180);
        btnCancel = CreateButton("İsteği İptal Et", Color.FromArgb(220, 38, 38), 0, 0, 150);
        InitializeShowcase();
    }

    private void InitializeShowcase()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(248, 250, 252);
        Padding = new Padding(24);

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 116,
            BackColor = Color.White,
            Padding = new Padding(22)
        };

        topPanel.Controls.Add(new Label
        {
            Text = "Kitap vitrini",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(22, 14),
            AutoSize = true
        });

        topPanel.Controls.Add(new Label
        {
            Text = "Kapak kartlarından kitap seç; sağdaki detay bölümünden özetini, ISBN bilgisini ve müsaitliğini gör.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(24, 48),
            Size = new Size(850, 24)
        });

        txtSearch.Location = new Point(24, 78);
        txtSearch.Size = new Size(360, 28);
        txtSearch.PlaceholderText = "Kitap, yazar, tür, ISBN veya yayınevi ara";
        txtSearch.TextChanged += (_, _) => LoadBooks();
        topPanel.Controls.Add(txtSearch);

        lblStatus.Location = new Point(405, 82);
        lblStatus.Size = new Size(580, 24);
        lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
        topPanel.Controls.Add(lblStatus);

        var requestPanel = CreateRequestPanel();
        requestPanel.Dock = DockStyle.Bottom;
        requestPanel.Height = 185;

        var mainContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(0, 12, 0, 12)
        };

        var detailPanel = CreateDetailPanel();
        detailPanel.Dock = DockStyle.Right;
        detailPanel.Width = 410;
        detailPanel.Margin = new Padding(12, 0, 0, 0);

        var galleryPanel = CreateBookGalleryPanel();
        galleryPanel.Dock = DockStyle.Fill;

        mainContent.SizeChanged += (_, _) =>
        {
            detailPanel.Width = Math.Max(320, Math.Min(430, mainContent.Width / 3));
        };

        mainContent.Controls.Add(galleryPanel);
        mainContent.Controls.Add(detailPanel);

        Controls.Add(mainContent);
        Controls.Add(requestPanel);
        Controls.Add(topPanel);
        Load += (_, _) =>
        {
            RefreshAll();
        };
    }

    private Control CreateBookGalleryPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
        panel.Controls.Add(bookFlow);
        panel.Controls.Add(new Label
        {
            Text = "Kitaplar",
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        });

        bookFlow.Dock = DockStyle.Fill;
        bookFlow.AutoScroll = true;
        bookFlow.BackColor = Color.White;
        bookFlow.Padding = new Padding(2, 8, 2, 8);
        return panel;
    }

    private Control CreateDetailPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };

        lblCover.Dock = DockStyle.Top;
        lblCover.Height = 172;
        lblCover.TextAlign = ContentAlignment.MiddleCenter;
        lblCover.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        lblCover.ForeColor = Color.White;
        lblCover.Margin = new Padding(0, 0, 0, 12);

        lblDetailTitle.Dock = DockStyle.Top;
        lblDetailTitle.Height = 64;
        lblDetailTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblDetailTitle.ForeColor = Color.FromArgb(15, 23, 42);

        lblDetailMeta.Dock = DockStyle.Top;
        lblDetailMeta.Height = 72;
        lblDetailMeta.Font = new Font("Segoe UI", 9.5F);
        lblDetailMeta.ForeColor = Color.FromArgb(71, 85, 105);

        lblDetailStatus.Dock = DockStyle.Top;
        lblDetailStatus.Height = 34;
        lblDetailStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblDetailStatus.TextAlign = ContentAlignment.MiddleLeft;

        lblDetailAvailability.Dock = DockStyle.Top;
        lblDetailAvailability.Height = 54;
        lblDetailAvailability.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblDetailAvailability.ForeColor = Color.FromArgb(30, 41, 59);

        lblDetailIsbn.Dock = DockStyle.Top;
        lblDetailIsbn.Height = 44;
        lblDetailIsbn.Font = new Font("Segoe UI", 9.2F);
        lblDetailIsbn.ForeColor = Color.FromArgb(71, 85, 105);

        lblDetailSummary.Dock = DockStyle.Fill;
        lblDetailSummary.Font = new Font("Segoe UI", 9.5F);
        lblDetailSummary.ForeColor = Color.FromArgb(51, 65, 85);

        btnRent.Dock = DockStyle.Bottom;
        btnRent.Height = 38;
        btnRent.Click += (_, _) => RentSelected();

        panel.Controls.Add(lblDetailSummary);
        panel.Controls.Add(lblDetailIsbn);
        panel.Controls.Add(lblDetailAvailability);
        panel.Controls.Add(lblDetailStatus);
        panel.Controls.Add(lblDetailMeta);
        panel.Controls.Add(lblDetailTitle);
        panel.Controls.Add(lblCover);
        panel.Controls.Add(btnRent);
        return panel;
    }

    private Control CreateRequestPanel()
    {
        var panel = new Panel { BackColor = Color.White, Padding = new Padding(16), Margin = new Padding(0, 12, 0, 0) };
        var headerPanel = new Panel { Dock = DockStyle.Top, Height = 38 };
        headerPanel.Controls.Add(new Label
        {
            Text = "Bekleyen Kiralama İsteklerim",
            Dock = DockStyle.Left,
            Width = 310,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            TextAlign = ContentAlignment.MiddleLeft
        });
        btnCancel.Dock = DockStyle.Right;
        btnCancel.Width = 150;
        btnCancel.Click += (_, _) => CancelSelectedRequest();
        headerPanel.Controls.Add(btnCancel);

        lblRequestStatus.Dock = DockStyle.Bottom;
        lblRequestStatus.Height = 24;
        lblRequestStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRequestStatus.ForeColor = Color.FromArgb(71, 85, 105);

        ApplyGridStyle(requestGrid);
        panel.Controls.Add(requestGrid);
        panel.Controls.Add(lblRequestStatus);
        panel.Controls.Add(headerPanel);
        return panel;
    }

    private static Button CreateButton(string text, Color color, int left, int top, int width)
    {
        var button = new Button
        {
            Text = text,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(left, top),
            Size = new Size(width, 34),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static void ApplyGridStyle(DataGridView targetGrid)
    {
        targetGrid.Dock = DockStyle.Fill;
        targetGrid.ReadOnly = true;
        targetGrid.AllowUserToAddRows = false;
        targetGrid.AllowUserToDeleteRows = false;
        targetGrid.MultiSelect = false;
        targetGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        targetGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        targetGrid.BackgroundColor = Color.White;
        targetGrid.BorderStyle = BorderStyle.None;
        targetGrid.GridColor = Color.FromArgb(226, 232, 240);
        targetGrid.RowHeadersVisible = false;
        targetGrid.EnableHeadersVisualStyles = false;
        targetGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
        targetGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        targetGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        targetGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
        targetGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        targetGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
        targetGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        targetGrid.RowTemplate.Height = 30;
    }

    private void RefreshAll()
    {
        LoadBooks();
        LoadRequests();
    }

    private void LoadBooks()
    {
        books = DatabaseHelper.GetAvailableBooksForShowcase(txtSearch.Text.Trim());
        bookFlow.SuspendLayout();
        bookFlow.Controls.Clear();
        selectedBook = null;
        selectedCard = null;

        foreach (DataRow row in books.Rows)
        {
            bookFlow.Controls.Add(CreateBookCard(row));
        }

        bookFlow.ResumeLayout();
        lblStatus.Text = $"Gösterilen kitap: {books.Rows.Count}";

        if (books.Rows.Count > 0)
        {
            SelectBook(books.Rows[0], bookFlow.Controls.OfType<Panel>().FirstOrDefault());
        }
        else
        {
            ClearDetail();
        }
    }

    private Panel CreateBookCard(DataRow row)
    {
        var title = GetText(row, "Kitap Adı");
        var authors = GetText(row, "Yazarlar");
        var genres = GetText(row, "Türler");
        var availableCopies = GetInt(row, "MusaitKopya");
        var totalCopies = GetInt(row, "ToplamKopya");
        var status = GetText(row, "Müsaitlik");
        var coverColor = GetCoverColor(title);
        var coverImage = GetCoverImage(row);

        var card = new Panel
        {
            Width = 218,
            Height = 272,
            BackColor = Color.White,
            Margin = new Padding(8),
            Padding = new Padding(10),
            BorderStyle = BorderStyle.FixedSingle,
            Tag = row,
            Cursor = Cursors.Hand
        };

        var cover = new Label
        {
            Dock = DockStyle.Top,
            Height = 112,
            BackColor = coverImage is null ? coverColor : Color.White,
            ForeColor = Color.White,
            Text = coverImage is null ? BuildCoverText(title) : string.Empty,
            BackgroundImage = coverImage,
            BackgroundImageLayout = ImageLayout.Zoom,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 46,
            Text = title,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Cursor = Cursors.Hand
        };

        var authorLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 38,
            Text = string.IsNullOrWhiteSpace(authors) ? "Yazar bilgisi yok" : authors,
            Font = new Font("Segoe UI", 8.8F),
            ForeColor = Color.FromArgb(71, 85, 105),
            Cursor = Cursors.Hand
        };

        var genreLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 28,
            Text = string.IsNullOrWhiteSpace(genres) ? "Tür belirtilmemiş" : genres,
            Font = new Font("Segoe UI", 8.6F),
            ForeColor = Color.FromArgb(100, 116, 139),
            Cursor = Cursors.Hand
        };

        var statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 30,
            Text = status == "Müsait" ? $"Müsait • {availableCopies}/{totalCopies}" : status,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = availableCopies > 0 ? Color.FromArgb(22, 101, 52) : Color.FromArgb(185, 28, 28),
            BackColor = availableCopies > 0 ? Color.FromArgb(220, 252, 231) : Color.FromArgb(254, 226, 226),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };

        card.Controls.Add(statusLabel);
        card.Controls.Add(genreLabel);
        card.Controls.Add(authorLabel);
        card.Controls.Add(titleLabel);
        card.Controls.Add(cover);
        WireCardClick(card, card, row);
        return card;
    }

    private void WireCardClick(Control control, Panel card, DataRow row)
    {
        control.Click += (_, _) => SelectBook(row, card);
        foreach (Control child in control.Controls)
        {
            WireCardClick(child, card, row);
        }
    }

    private void SelectBook(DataRow row, Panel? card)
    {
        selectedCard?.SetBounds(selectedCard.Left, selectedCard.Top, selectedCard.Width, selectedCard.Height);
        if (selectedCard is not null)
        {
            selectedCard.BackColor = Color.White;
        }

        selectedBook = row;
        selectedCard = card;
        if (selectedCard is not null)
        {
            selectedCard.BackColor = Color.FromArgb(239, 246, 255);
        }

        var title = GetText(row, "Kitap Adı");
        var authors = GetText(row, "Yazarlar");
        var publisher = GetText(row, "Yayınevi");
        var language = GetText(row, "Dil");
        var genres = GetText(row, "Türler");
        var year = GetText(row, "İlk Yayın Yılı");
        var isbn = GetText(row, "ISBN");
        var summary = GetText(row, "Özet");
        var availableCopies = GetInt(row, "MusaitKopya");
        var totalCopies = GetInt(row, "ToplamKopya");
        var rentedCopies = GetInt(row, "KiradaKopya");
        var coverImage = GetCoverImage(row);

        lblCover.BackColor = coverImage is null ? GetCoverColor(title) : Color.White;
        lblCover.BackgroundImage = coverImage;
        lblCover.BackgroundImageLayout = ImageLayout.Zoom;
        lblCover.Text = coverImage is null ? BuildCoverText(title) : string.Empty;
        lblDetailTitle.Text = title;
        lblDetailMeta.Text = $"Yazar: {ValueOrDash(authors)}\nTür: {ValueOrDash(genres)}\nYayınevi: {ValueOrDash(publisher)} • Dil: {ValueOrDash(language)} • Yıl: {ValueOrDash(year)}";
        lblDetailStatus.Text = availableCopies > 0 ? "● Şu anda kiralanabilir" : "● Şu anda müsait kopya yok";
        lblDetailStatus.ForeColor = availableCopies > 0 ? Color.FromArgb(22, 101, 52) : Color.FromArgb(185, 28, 28);
        lblDetailAvailability.Text = $"Müsait: {availableCopies}    Kirada/Rezerve: {rentedCopies}    Toplam: {totalCopies}";
        lblDetailIsbn.Text = $"ISBN: {ValueOrDash(isbn)}";
        lblDetailSummary.Text = string.IsNullOrWhiteSpace(summary)
            ? "Bu kitap için özet bilgisi eklenmemiş."
            : $"Özet\n{summary}";
        btnRent.Enabled = availableCopies > 0;
        btnRent.BackColor = availableCopies > 0 ? Color.FromArgb(37, 99, 235) : Color.FromArgb(148, 163, 184);
    }

    private void ClearDetail()
    {
        lblCover.BackColor = Color.FromArgb(148, 163, 184);
        lblCover.Text = "?";
        lblDetailTitle.Text = "Kitap bulunamadı";
        lblDetailMeta.Text = "Arama kriterini değiştirerek yeniden deneyebilirsin.";
        lblDetailStatus.Text = string.Empty;
        lblDetailAvailability.Text = string.Empty;
        lblDetailIsbn.Text = string.Empty;
        lblDetailSummary.Text = string.Empty;
        btnRent.Enabled = false;
        btnRent.BackColor = Color.FromArgb(148, 163, 184);
    }

    private void LoadRequests()
    {
        if (currentUser.RecordId is null)
        {
            return;
        }

        requestGrid.DataSource = DatabaseHelper.GetUserRentalRequests(currentUser.RecordId.Value);
        var idColumn = requestGrid.Columns["RezervasyonID"];
        if (idColumn is not null)
        {
            idColumn.Visible = false;
        }

        lblRequestStatus.Text = $"Bekleyen istek: {requestGrid.Rows.Count}";
    }

    private void RentSelected()
    {
        if (currentUser.RecordId is null || selectedBook is null)
        {
            MessageBox.Show("Kiralama için kitap seçin.", "Kitaplar");
            return;
        }

        if (GetInt(selectedBook, "MusaitKopya") <= 0)
        {
            MessageBox.Show("Bu kitap için müsait kopya yok.", "Kitaplar");
            return;
        }

        try
        {
            DatabaseHelper.CreateRentalRequest(currentUser.RecordId.Value, Convert.ToInt32(selectedBook["KitapID"]));
            lblStatus.Text = "Kiralama isteği personele gönderildi.";
            RefreshAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kitaplar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CancelSelectedRequest()
    {
        if (currentUser.RecordId is null || requestGrid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            MessageBox.Show("İptal etmek için bekleyen bir istek seçin.", "Kitaplar");
            return;
        }

        var result = MessageBox.Show("Seçili kiralama isteği iptal edilsin mi?", "İstek İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            DatabaseHelper.CancelRentalRequest(currentUser.RecordId.Value, Convert.ToInt32(row["RezervasyonID"]));
            lblStatus.Text = "Kiralama isteği iptal edildi.";
            RefreshAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Kitaplar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string GetText(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
            ? Convert.ToString(row[columnName])?.Trim() ?? string.Empty
            : string.Empty;
    }

    private static int GetInt(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
            ? Convert.ToInt32(row[columnName])
            : 0;
    }

    private static string ValueOrDash(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }

    private static string BuildCoverText(string title)
    {
        var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (words.Length == 0)
        {
            return "K";
        }

        return string.Concat(words.Take(2).Select(word => char.ToUpper(word[0])));
    }

    private static Color GetCoverColor(string title)
    {
        var palette = new[]
        {
            Color.FromArgb(37, 99, 235),
            Color.FromArgb(124, 58, 237),
            Color.FromArgb(15, 118, 110),
            Color.FromArgb(194, 65, 12),
            Color.FromArgb(190, 24, 93),
            Color.FromArgb(30, 64, 175),
            Color.FromArgb(22, 101, 52)
        };
        var hash = Math.Abs(title.Aggregate(17, (current, character) => current * 31 + character));
        return palette[hash % palette.Length];
    }

    private Image? GetCoverImage(DataRow row)
    {
        var kitapId = GetInt(row, "KitapID");
        if (kitapId <= 0)
        {
            return null;
        }

        if (coverCache.TryGetValue(kitapId, out var cachedImage))
        {
            return cachedImage;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "BookCovers", $"{kitapId}.jpg");
        if (!File.Exists(path))
        {
            coverCache[kitapId] = null;
            return null;
        }

        using var stream = File.OpenRead(path);
        using var image = Image.FromStream(stream);
        var bitmap = new Bitmap(image);
        coverCache[kitapId] = bitmap;
        return bitmap;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var image in coverCache.Values)
            {
                image?.Dispose();
            }
        }

        base.Dispose(disposing);
    }
}
