using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace bai4._6
{
    public partial class Form1 : Form
    {
        private readonly BindingList<Student> _students = new();
        private readonly BindingSource _studentSource = new();
        private readonly DataGridView _studentGrid = new();
        private readonly TextBox _studentIdTextBox = new();
        private readonly TextBox _fullNameTextBox = new();
        private readonly TextBox _ageTextBox = new();
        private readonly ComboBox _genderComboBox = new();
        private readonly PictureBox _avatarPictureBox = new();
        private readonly Button _chooseAvatarButton = new();
        private readonly Button _addButton = new();
        private readonly Button _deleteButton = new();
        private readonly Button _updateButton = new();
        private readonly Button _exportButton = new();

        public Form1()
        {
            InitializeComponent();
            InitializeStudentForm();
            ConfigureBindings();
            _addButton.Click += AddStudent;
            _deleteButton.Click += DeleteStudent;
            _updateButton.Click += UpdateStudent;
            _chooseAvatarButton.Click += ChooseAvatar;
            _exportButton.Click += ExportCsv;
        }

        private void InitializeStudentForm()
        {
            Text = "Quản lý sinh viên";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);
            ClientSize = new Size(1100, 700);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 185));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            Controls.Add(mainLayout);

            var editorLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 8)
            };
            editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            mainLayout.Controls.Add(editorLayout, 0, 0);

            var fieldsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            AddField(fieldsLayout, "Mã sinh viên", _studentIdTextBox, 0, 0);
            AddField(fieldsLayout, "Họ và tên", _fullNameTextBox, 2, 0);
            AddField(fieldsLayout, "Tuổi", _ageTextBox, 0, 1);
            AddField(fieldsLayout, "Giới tính", _genderComboBox, 2, 1);
            _genderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _genderComboBox.Items.AddRange(new object[] { "Nam", "Nữ" });
            editorLayout.Controls.Add(fieldsLayout, 0, 0);

            var avatarLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10, 0, 0, 0)
            };
            avatarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            avatarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            _avatarPictureBox.Dock = DockStyle.Fill;
            _avatarPictureBox.BorderStyle = BorderStyle.FixedSingle;
            _avatarPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            _chooseAvatarButton.Text = "Chọn ảnh đại diện...";
            _chooseAvatarButton.Dock = DockStyle.Fill;
            avatarLayout.Controls.Add(_avatarPictureBox, 0, 0);
            avatarLayout.Controls.Add(_chooseAvatarButton, 0, 1);
            editorLayout.Controls.Add(avatarLayout, 1, 0);

            _studentGrid.Dock = DockStyle.Fill;
            _studentGrid.AllowUserToAddRows = false;
            _studentGrid.AllowUserToDeleteRows = false;
            _studentGrid.AutoGenerateColumns = false;
            _studentGrid.MultiSelect = false;
            _studentGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _studentGrid.ReadOnly = false;
            _studentGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _studentGrid.BackgroundColor = Color.White;
            _studentGrid.GridColor = Color.Gainsboro;
            _studentGrid.RowHeadersWidth = 42;
            _studentGrid.ColumnHeadersHeight = 34;
            _studentGrid.RowTemplate.Height = 30;
            _studentGrid.Columns.Add(CreateColumn(nameof(Student.StudentId), "Mã sinh viên", 20));
            _studentGrid.Columns.Add(CreateColumn(nameof(Student.FullName), "Họ và tên", 45));
            _studentGrid.Columns.Add(CreateColumn(nameof(Student.Age), "Tuổi", 15));
            _studentGrid.Columns.Add(CreateColumn(nameof(Student.Gender), "Giới tính", 20));
            mainLayout.Controls.Add(_studentGrid, 0, 1);

            var actionsLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0),
                WrapContents = false
            };
            ConfigureActionButton(_exportButton, "Xuất CSV", 120);
            ConfigureActionButton(_updateButton, "Cập nhật", 110);
            ConfigureActionButton(_deleteButton, "Xóa", 90);
            ConfigureActionButton(_addButton, "Thêm", 90);
            actionsLayout.Controls.AddRange(new Control[] { _exportButton, _updateButton, _deleteButton, _addButton });
            mainLayout.Controls.Add(actionsLayout, 0, 2);
        }

        private void ConfigureBindings()
        {
            _studentSource.DataSource = _students;
            _studentSource.AllowNew = false;
            _studentGrid.DataSource = _studentSource;

            _studentIdTextBox.DataBindings.Add(nameof(TextBox.Text), _studentSource, nameof(Student.StudentId), true, DataSourceUpdateMode.OnPropertyChanged);
            _fullNameTextBox.DataBindings.Add(nameof(TextBox.Text), _studentSource, nameof(Student.FullName), true, DataSourceUpdateMode.OnPropertyChanged);
            var ageBinding = new Binding(nameof(TextBox.Text), _studentSource, nameof(Student.Age), true, DataSourceUpdateMode.OnValidation);
            ageBinding.Format += (_, e) => e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) ?? string.Empty;
            ageBinding.Parse += (_, e) =>
            {
                e.Value = int.TryParse(Convert.ToString(e.Value, CultureInfo.CurrentCulture), out var age) ? age : 0;
            };
            _ageTextBox.DataBindings.Add(ageBinding);
            _genderComboBox.DataBindings.Add(nameof(ComboBox.SelectedItem), _studentSource, nameof(Student.Gender), true, DataSourceUpdateMode.OnPropertyChanged);
            _avatarPictureBox.DataBindings.Add(nameof(PictureBox.ImageLocation), _studentSource, nameof(Student.AvatarPath), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private static void AddField(TableLayoutPanel layout, string labelText, Control input, int column, int row)
        {
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            input.Dock = DockStyle.Fill;
            if (input is TextBox textBox)
            {
                textBox.Margin = new Padding(4, 12, 8, 4);
            }

            layout.Controls.Add(label, column, row);
            layout.Controls.Add(input, column + 1, row);
        }

        private static DataGridViewTextBoxColumn CreateColumn(string propertyName, string headerText, int fillWeight)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                HeaderText = headerText,
                Name = propertyName,
                FillWeight = fillWeight,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
        }

        private static void ConfigureActionButton(Button button, string text, int width)
        {
            button.Text = text;
            button.Width = width;
            button.Height = 34;
            button.Margin = new Padding(6, 0, 0, 0);
        }

        private void AddStudent(object? sender, EventArgs e)
        {
            _studentSource.EndEdit();
            var student = new Student();
            _students.Add(student);
            _studentSource.Position = _students.Count - 1;
            _studentIdTextBox.Focus();
        }

        private void DeleteStudent(object? sender, EventArgs e)
        {
            if (_studentSource.Current is not Student student)
            {
                MessageBox.Show(this, "Vui lòng chọn sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(this, $"Bạn có chắc muốn xóa sinh viên {student.FullName}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                _studentSource.RemoveCurrent();
            }
        }

        private void UpdateStudent(object? sender, EventArgs e)
        {
            if (_studentSource.Current is not Student student)
            {
                MessageBox.Show(this, "Vui lòng chọn hoặc thêm sinh viên trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(_studentIdTextBox.Text) || string.IsNullOrWhiteSpace(_fullNameTextBox.Text))
            {
                MessageBox.Show(this, "Mã sinh viên và họ tên không được để trống.", "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(_ageTextBox.Text, out var age) || age < 0)
            {
                MessageBox.Show(this, "Tuổi phải là số nguyên không âm.", "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _ageTextBox.Focus();
                return;
            }

            _studentGrid.EndEdit();
            _studentSource.EndEdit();
            student.Age = age;
            _studentSource.ResetCurrentItem();
            MessageBox.Show(this, "Thông tin sinh viên đã được cập nhật.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChooseAvatar(object? sender, EventArgs e)
        {
            if (_studentSource.Current is not Student student)
            {
                MessageBox.Show(this, "Vui lòng chọn hoặc thêm sinh viên trước khi chọn ảnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh đại diện",
                Filter = "Tệp hình ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Tất cả tệp (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                student.AvatarPath = dialog.FileName;
                _studentSource.ResetCurrentItem();
            }
        }

        private void ExportCsv(object? sender, EventArgs e)
        {
            _studentGrid.EndEdit();
            _studentSource.EndEdit();

            using var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách sinh viên",
                Filter = "Tệp CSV (*.csv)|*.csv",
                DefaultExt = "csv",
                AddExtension = true,
                FileName = "DanhSachSinhVien.csv",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                var lines = new List<string> { "StudentId,FullName,Age,Gender,AvatarPath" };
                lines.AddRange(_students.Select(student => string.Join(",", new[]
                {
                    EscapeCsv(student.StudentId),
                    EscapeCsv(student.FullName),
                    student.Age.ToString(CultureInfo.InvariantCulture),
                    EscapeCsv(student.Gender),
                    EscapeCsv(student.AvatarPath)
                })));

                File.WriteAllLines(dialog.FileName, lines, new UTF8Encoding(true));
                MessageBox.Show(this, "Đã xuất danh sách sinh viên ra tệp CSV.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Không thể ghi tệp CSV: {exception.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscapeCsv(string? value)
        {
            var escaped = (value ?? string.Empty).Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }
    }
}
