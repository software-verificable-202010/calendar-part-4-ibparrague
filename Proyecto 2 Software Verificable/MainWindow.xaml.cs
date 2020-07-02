using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit;

[assembly: CLSCompliant(true)]
namespace Proyecto4SoftwareVerificable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        #region Constants
        private const int mondayPosition = 0;
        private const int tuesdayPosition = 1;
        private const int wednesdayPosition = 2;
        private const int thursdayPosition = 3;
        private const int fridayPosition = 4;
        private const int saturdayPosition = 5;
        private const int sundayPosition = 6;

        private const int hoursInADay = 24;
        private const int minutesInADay = 1440;

        private const string mondayName = "Monday";
        private const string tuesdayName = "Tuesday";
        private const string wednesdayName = "Wednesday";
        private const string thursdayName = "Thursday";
        private const string fridayName = "Friday";
        private const string saturdayName = "Saturday";
        private const string sundayName = "Sunday";

        private const string serializedAppointmentFileName = "SeriaizedAppointments.txt";
        private bool isArrowInputAvailable = true;
        #endregion

        #region Fields
        private readonly List<Appointment> appointments = new List<Appointment>();
        private readonly List<User> users = new List<User>();
        private DateTime activeMonthDate = DateTime.Now;
        private DateTime activeWeekDate = DateTime.Now;
        private User currentUser = new User();
        private Appointment targetAppoitnemnt = new Appointment();
        private GridsToReturnFromAppointment gridToReturnFromAppointment;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            appointments = DeserializeAppointments(serializedAppointmentFileName);
            string serializedUsersFileName = "SerializedUsers.txt";
            users = DeserializeUsers(serializedUsersFileName);
            this.KeyDown += new KeyEventHandler(ReadArrowInput);
        }
        #endregion

        #region Properties
        //Properties are only used in UnitTests.
        public DateTime ActiveMonthDate
        {
            get
            {
                return activeMonthDate;
            }
            set
            {
                activeMonthDate = value;
            }
        }
        #endregion

        #region Methods
        public enum CustomRectangleStyle
        {
            blueLines,
            grayLines,
        }
        public enum GridsToReturnFromAppointment
        {
            MonthlyView,
            WeeklyView,
        }
        public static DateTime GetFirstDayOfWeek(DateTime dateOfReference)
        {
            string dayOfWeekOfReferenceDate = dateOfReference.DayOfWeek.ToString();
            int daysToSubtractToGetFirstDayOfWeek = 0;
            switch (dayOfWeekOfReferenceDate)
            {
                case mondayName:
                    daysToSubtractToGetFirstDayOfWeek = 0;
                    break;
                case tuesdayName:
                    daysToSubtractToGetFirstDayOfWeek = 1;
                    break;
                case wednesdayName:
                    daysToSubtractToGetFirstDayOfWeek = 2;
                    break;
                case thursdayName:
                    daysToSubtractToGetFirstDayOfWeek = 3;
                    break;
                case fridayName:
                    daysToSubtractToGetFirstDayOfWeek = 4;
                    break;
                case saturdayName:
                    daysToSubtractToGetFirstDayOfWeek = 5;
                    break;
                case sundayName:
                    daysToSubtractToGetFirstDayOfWeek = 6;
                    break;
            }
            DateTime firstDayOfWeek = dateOfReference.AddDays(-daysToSubtractToGetFirstDayOfWeek);
            return firstDayOfWeek;
        }
        public static void SerializeAppointments(List<Appointment> appointments, string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(fileName);
            string appointmentsFilePath = stringBuilder.ToString();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(appointmentsFilePath, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, appointments);
            stream.Close();
        }
        public static List<Appointment> DeserializeAppointments(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(fileName);
            string appointmentsFilePath = stringBuilder.ToString();
            if (File.Exists(appointmentsFilePath))
            {
                Stream stream = new FileStream(appointmentsFilePath, FileMode.Open, FileAccess.Read);
                List<Appointment> loadedAppointments = formatter.Deserialize(stream) as List<Appointment>;
                stream.Close();
                return loadedAppointments;
            }
            else
            {
                return new List<Appointment>();
            }
        }
        public static void SerializeUsers(List<User> users, string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(fileName);
            string usersFilePath = stringBuilder.ToString();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(usersFilePath, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, users);
            stream.Close();
        }
        public static List<User> DeserializeUsers(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(fileName);
            string usersFilePath = stringBuilder.ToString();
            if (File.Exists(usersFilePath))
            {
                Stream stream = new FileStream(usersFilePath, FileMode.Open, FileAccess.Read);
                List<User> loadedUsers = formatter.Deserialize(stream) as List<User>;
                stream.Close();
                return loadedUsers;
            }
            else
            {
                return new List<User>();
            }
        }
        public static Rectangle CustomRectangle(CustomRectangleStyle customRectangleStyle)
        {
            Rectangle rectangle = new Rectangle();
            if (customRectangleStyle == CustomRectangleStyle.blueLines)
            {
                var transparentBrush = Brushes.Transparent;
                var blueStroke = new BrushConverter().ConvertFrom("#1f3861");
                SolidColorBrush strokeBrush = blueStroke as SolidColorBrush;
                double strokeThickness = 2;

                rectangle = new Rectangle()
                {
                    Fill = transparentBrush,
                    Stroke = strokeBrush,
                    StrokeThickness = strokeThickness
                };
            }
            else if (customRectangleStyle == CustomRectangleStyle.grayLines)
            {
                var transparentBrush = Brushes.Transparent;
                var grayStroke = new BrushConverter().ConvertFrom("#999999");
                SolidColorBrush strokeBrush = grayStroke as SolidColorBrush;
                double strokeThickness = 0.5;
                rectangle = new Rectangle()
                {
                    Fill = transparentBrush,
                    Stroke = strokeBrush,
                    StrokeThickness = strokeThickness
                };
            }
            return rectangle;
        }
        public static Label GenerateAppointmentTitleLabel(double topMargin, double bottomMargin, Appointment validAppointment)
        {
            if (validAppointment == null)
            {
                throw new ArgumentNullException(nameof(validAppointment));
            }
            StringBuilder labelContent = new StringBuilder();
            int maxHorizontalCharacterForLabelTitle = 14;
            int index = 0;
            Label titleLabel = new Label
            {
                Margin = new Thickness(0, topMargin, 0, bottomMargin),
                FontSize = 20,
                Content = "",
            };
            string titleForLabel = validAppointment.Title;
            while (index < titleForLabel.Length)
            {
                labelContent.Append(validAppointment.Title[index]);
                bool isTitleMaxLenght = index % maxHorizontalCharacterForLabelTitle == 0;
                bool isTitlePastFirstLetter = index > 1;
                bool isValid = isTitleMaxLenght && isTitlePastFirstLetter;
                if (isValid)
                {
                    labelContent.Append("\n");
                }
                index++;
            }
            titleLabel.Content = labelContent;
            return titleLabel;


        }
        public static int GetXCoordinateOfDay(DateTime date)
        {
            int xCoordinateToInsertNumber = 0;
            string dayOfWeekToStart = date.DayOfWeek.ToString();
            switch (dayOfWeekToStart)
            {
                case mondayName:
                    xCoordinateToInsertNumber = mondayPosition;
                    break;
                case tuesdayName:
                    xCoordinateToInsertNumber = tuesdayPosition;
                    break;
                case wednesdayName:
                    xCoordinateToInsertNumber = wednesdayPosition;
                    break;
                case thursdayName:
                    xCoordinateToInsertNumber = thursdayPosition;
                    break;
                case fridayName:
                    xCoordinateToInsertNumber = fridayPosition;
                    break;
                case saturdayName:
                    xCoordinateToInsertNumber = saturdayPosition;
                    break;
                case sundayName:
                    xCoordinateToInsertNumber = sundayPosition;
                    break;
            }
            return xCoordinateToInsertNumber;
        }
        public static Label GenerateAppointDescriptionLabel(double topMargin, int titleTextLines, double bottomMargin, Appointment validAppointment)
        {
            if (validAppointment == null)
            {
                throw new ArgumentNullException(nameof(validAppointment));
            }
            int titleFontLinePixelsSize = 25;
            int maxHorizontalCharacterForLabelDescription = 14;
            Label descriptionLabel = new Label
            {
                Margin = new Thickness(0, topMargin + (titleFontLinePixelsSize * titleTextLines), 0, bottomMargin),
                FontSize = 10,
                Content = "",
            };
            string desciptionForLabel = validAppointment.Description;
            StringBuilder labelContent = new StringBuilder();
            int index = 0;
            while (index < desciptionForLabel.Length)
            {
                labelContent.Append(validAppointment.Description[index]);
                bool isDescriptionMaxLenght = index % maxHorizontalCharacterForLabelDescription == 0;
                bool isDescriptionPastFirstLetter = index > 1;
                bool isValid = isDescriptionMaxLenght && isDescriptionPastFirstLetter;
                if (isValid)
                {
                    labelContent.Append("\n");
                }
                index++;
            }
            descriptionLabel.Content = labelContent;
            return descriptionLabel;
        }
        public static Rectangle GenerateRectangleForAppointment(Random randomizer, double rectangleHeight, double topMargin, double bottomMargin, int xCoordinateToInsert, int weekGridOffset)
        {
            if (randomizer == null)
            {
                throw new ArgumentNullException(nameof(randomizer));
            }
            Color randomColor = GenerateRandomColor(randomizer);
            SolidColorBrush randomColorBrush = new SolidColorBrush(randomColor);
            Rectangle rectangle = new Rectangle
            {
                Height = rectangleHeight,
                Margin = new Thickness(0, topMargin, 0, bottomMargin),
                Fill = Brushes.Transparent,
                StrokeThickness = 5,
                Stroke = randomColorBrush,
            };
            Grid.SetColumn(rectangle, xCoordinateToInsert + weekGridOffset);
            Grid.SetRowSpan(rectangle, hoursInADay);
            return rectangle;
        }
        public static void AddColumnsToAppointmentGrid(Grid gridAppointment)
        {
            if (gridAppointment == null)
            {
                throw new ArgumentNullException(nameof(gridAppointment));
            }
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(3, GridUnitType.Star);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(3, GridUnitType.Star);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(3, GridUnitType.Star);
            ColumnDefinition column4 = new ColumnDefinition();
            column4.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition column5 = new ColumnDefinition();
            column4.Width = new GridLength(1, GridUnitType.Star);
            gridAppointment.ColumnDefinitions.Add(column1);
            gridAppointment.ColumnDefinitions.Add(column2);
            gridAppointment.ColumnDefinitions.Add(column3);
            gridAppointment.ColumnDefinitions.Add(column4);
            gridAppointment.ColumnDefinitions.Add(column5);
        }
        public static void AddLabelsToAppointmentGrid(Appointment appointment, Grid gridAppointment)
        {
            if (gridAppointment == null)
            {
                throw new ArgumentNullException(nameof(gridAppointment));
            }
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }
            Label labelTitle = new Label();
            Label labelDescription = new Label();
            Label labelDate = new Label();

            labelTitle.Content = appointment.Title;
            labelDescription.Content = appointment.Description;
            labelDate.Content = appointment.Date.ToString(CultureInfo.CurrentCulture);

            labelTitle.Width = 384;
            labelDescription.Width = 384;
            labelDate.Width = 384;

            labelTitle.FontSize = 20;
            labelDescription.FontSize = 20;
            labelDate.FontSize = 20;

            Grid.SetColumn(labelTitle, 0);
            Grid.SetColumn(labelDescription, 1);
            Grid.SetColumn(labelDate, 2);

            gridAppointment.Children.Add(labelTitle);
            gridAppointment.Children.Add(labelDescription);
            gridAppointment.Children.Add(labelDate);
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            activeMonthDate = activeMonthDate.AddMonths(1);
            UpdateCalendarGrid(activeMonthDate);
        }
        private void BtnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            activeMonthDate = activeMonthDate.AddMonths(-1);
            UpdateCalendarGrid(activeMonthDate);
        }
        private void BtnSaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (CheckifAppointmentIsValid())
            {
                SaveAppointment();

                if (gridToReturnFromAppointment == GridsToReturnFromAppointment.MonthlyView)
                {
                    DisplayMonthMenu();
                    UpdateCalendarGrid(activeMonthDate);
                }
                else if (gridToReturnFromAppointment == GridsToReturnFromAppointment.WeeklyView)
                {
                    DisplayWeeklyViewMenu();
                    UpdateCalendarWeekGrid();
                }
                SerializeAppointments(appointments, serializedAppointmentFileName);
            }
            else
            {
                lblErrorsNewAppointment.Content = "Error creating appointment, check fields";
            }
        }
        private void BtnAcceptLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtLoginUsername.Text.Length > 0)
            {
                string username = txtLoginUsername.Text;
                ManageUserData(username);
                UpdateCalendarGrid(activeMonthDate);
                DisplayMonthMenu();
            }
            else
            {
                lblLoginError.Content = "Error, check fields!";
            }
        }
        private void BtnGoBackToMonth_Click(object sender, RoutedEventArgs e)
        {
            DisplayMonthMenu();
            UpdateCalendarGrid(activeMonthDate);
        }
        private void BtnCancelNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (gridToReturnFromAppointment == GridsToReturnFromAppointment.MonthlyView)
            {
                UpdateCalendarGrid(activeMonthDate);
                DisplayMonthMenu();
            }
            else if (gridToReturnFromAppointment == GridsToReturnFromAppointment.WeeklyView)
            {
                DisplayWeeklyViewMenu();
            }
        }
        private void BtnPreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            int daysInAWeek = 7;
            activeWeekDate = activeWeekDate.AddDays(-daysInAWeek);
            UpdateCalendarWeekGrid();
        }
        private void BtnEditAppointmentMonth_Click(object sender, RoutedEventArgs e)
        {
            DisplayAppointmentsList();
            UpdateAppointmentsList();
        }
        private void BtnNextWeek_Click(object sender, RoutedEventArgs e)
        {
            int daysInAWeek = 7;
            activeWeekDate = activeWeekDate.AddDays(daysInAWeek);
            UpdateCalendarWeekGrid();
        }
        private void ReadArrowInput(object sender, KeyEventArgs e)
        {
            if (isArrowInputAvailable)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        activeMonthDate = activeMonthDate.AddMonths(1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Left:
                        activeMonthDate = activeMonthDate.AddMonths(-1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Up:
                        activeMonthDate = activeMonthDate.AddYears(1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Down:
                        activeMonthDate = activeMonthDate.AddYears(-1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                }
            }
        }
        private void UpdateCalendarGrid(DateTime dateOfReference)
        {
            gridCalendar.Children.Clear();
            UpdateMonthYearLabel(dateOfReference);
            FillGridCalendar(gridCalendar, dateOfReference);
            CreateClickableGridBorders(gridCalendar, CustomRectangleStyle.blueLines);
        }
        private void RectangleDayBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = e.Source as Rectangle;
            if (gridCalendar.Children.Contains(rectangle))
            {
                int index = Grid.GetRow(rectangle);
                int weekRowClicked = index;
                DateTime firstDayOfMonth = activeMonthDate.Date.AddDays(1 - DateTime.Now.Day);
                int daysInAWeek = 7;
                activeWeekDate = firstDayOfMonth.AddDays(daysInAWeek * weekRowClicked);
                UpdateCalendarWeekGrid();
                DisplayWeeklyViewMenu();
            }
        }
        private void UpdateCalendarWeekGrid()
        {
            ClearWeekGrids();
            UpdateWeekMonthLabel(activeWeekDate);
            FillGridWeeklyDaysLabels(activeWeekDate);
            FillGridWeekHours(gridDayHoursNumbers);
            FillGridWeekAppointments(activeWeekDate);
            CreateClickableGridBorders(gridDayHoursNumbers, CustomRectangleStyle.grayLines);
        }
        private void CreateClickableGridBorders(Grid grid, CustomRectangleStyle customRectangleStyle)
        {
            if (grid != null)
            {
                int columnAmount = grid.ColumnDefinitions.Count;
                int rowAmount = grid.RowDefinitions.Count;
                int xCoordinate = 0;
                int yCoordinate = 0;
                while (yCoordinate < rowAmount)
                {
                    xCoordinate = 0;
                    while (xCoordinate < columnAmount)
                    {
                        Rectangle rectangleDayBorder = CustomRectangle(customRectangleStyle);
                        rectangleDayBorder.MouseDown += RectangleDayBorder_MouseDown;
                        Grid.SetColumn(rectangleDayBorder, xCoordinate);
                        Grid.SetRow(rectangleDayBorder, yCoordinate);
                        grid.Children.Add(rectangleDayBorder);
                        xCoordinate++;
                    }
                    yCoordinate++;
                }
            }
        }
        //Call dependency loop between the next four functions.
        private void UpdateAppointmentsList()
        {
            stackPanelAppointment.Children.Clear();
            List<Appointment> currentUserAppointments = appointments.FindAll(a => a.Creator.Name == currentUser.Name);
            foreach (Appointment app in currentUserAppointments)
            {
                AddAppointmentToStackPanel(app);
            }
        }
        private void AddAppointmentToStackPanel(Appointment appointment)
        {
            Grid gridAppointment = new Grid();
            gridAppointment.Height = 40;
            AddColumnsToAppointmentGrid(gridAppointment);
            AddLabelsToAppointmentGrid(appointment, gridAppointment);
            AddButtonsToAppointmentGrid(appointment, gridAppointment);
            stackPanelAppointment.Children.Add(gridAppointment);
        }
        private void AddButtonsToAppointmentGrid(Appointment appointment, Grid gridAppointment)
        {
            Button editButton = new Button();
            editButton.Content = "Edit";
            editButton.Click += (sender, EventArgs) => { EditButton_Click(appointment, this); };
            editButton.Width = 128;
            Grid.SetColumn(editButton, 3);

            Button deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Click += (sender, EventArgs) => { DeleteButton_Click(appointment, this); };
            deleteButton.Width = 128;
            Grid.SetColumn(deleteButton, 4);

            gridAppointment.Children.Add(editButton);
            gridAppointment.Children.Add(deleteButton);
        }
        private static void DeleteButton_Click(Appointment appointment, MainWindow mainWindow)
        {
            Appointment appointmentToDelete = mainWindow.targetAppoitnemnt;
            int indexToDelete = mainWindow.appointments.FindIndex(a => a == appointment);
            mainWindow.appointments.RemoveAt(indexToDelete);
            mainWindow.UpdateAppointmentsList();
            SerializeAppointments(mainWindow.appointments, serializedAppointmentFileName);
        }    
        private void ManageUserData(string username)
        {
            bool isUsernameUsed = users.Any(r => r.Name == username);
            if (isUsernameUsed)
            {
                currentUser = users.First(r => r.Name == username);
            }
            else
            {
                currentUser = new User(username);
                users.Add(currentUser);
                string serializedUserFileName = "SerializedUsers.txt";
                SerializeUsers(users, serializedUserFileName);
            }
        }
        private static void FillGridWeekHours(Grid grid)
        {
            if (grid != null)
            {
                int gridXCoordinate = 0;
                for (int index = 0; index < 24; index++)
                {
                    int gridYCoordinate = index;
                    Label labelHour = new Label()
                    {
                        FontSize = 20,
                        Foreground = (new BrushConverter().ConvertFrom("#999999")) as SolidColorBrush,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    StringBuilder labelHourContent = new StringBuilder();
                    labelHourContent.Append(index.ToString(CultureInfo.CurrentCulture));
                    labelHourContent.Append(":00");
                    labelHour.Content = labelHourContent;
                    Grid.SetColumn(labelHour, gridXCoordinate);
                    Grid.SetRow(labelHour, gridYCoordinate);
                    grid.Children.Add(labelHour);
                }
            }
        }
        private void UpdateMonthYearLabel(DateTime date)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            lblMonth.Content = date.ToString("MMMM", usEnglish).ToUpper(CultureInfo.CurrentCulture);
            lblYear.Content = date.Year.ToString(CultureInfo.CurrentCulture);
        }        
        private void FillGridCalendar(Grid gridCalendar, DateTime dateWithinAMonth)
        {
            if (gridCalendar != null)
            {
                int firstDayOfMonthNumber = 1;
                DateTime firstDayOfMonth = new DateTime(dateWithinAMonth.Year, dateWithinAMonth.Month, firstDayOfMonthNumber);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int yCoordinateToInsertNumber = 0;
                int numberOfDay = 1;
                int xCoordinateToInsertNumber = GetXCoordinateOfDay(firstDayOfMonth);
                while (numberOfDay <= lastDayOfMonth.Day)
                {
                    AddDifferentColourBackgroundsToCalendar(xCoordinateToInsertNumber, yCoordinateToInsertNumber);
                    AddLabelNumbersToEachDay(xCoordinateToInsertNumber, yCoordinateToInsertNumber, numberOfDay);
                    AddAppointmentToEachDay(xCoordinateToInsertNumber, yCoordinateToInsertNumber, firstDayOfMonth, numberOfDay);

                    xCoordinateToInsertNumber++;
                    if (xCoordinateToInsertNumber == gridCalendar.ColumnDefinitions.Count)
                    {
                        yCoordinateToInsertNumber++;
                        xCoordinateToInsertNumber = 0;
                    }

                    numberOfDay++;
                }
            }
        }
        private void FillGridWeekAppointments(DateTime dateOfReference)
        {
            DateTime mondayDate = GetFirstDayOfWeek(dateOfReference);
            const int numberDaysOfWeek = 7;
            DateTime sunday = mondayDate.AddDays(numberDaysOfWeek);
            List<Appointment> validWeekAppointments = appointments.FindAll(a => a.IsBetweenDates(mondayDate, sunday));
            List<Appointment> validWeekAndUserAppointments = validWeekAppointments.FindAll(a => a.IsUserInvited(currentUser));
            Random randomizer = new Random();
            foreach (Appointment validAppointment in validWeekAndUserAppointments)
            {
                int weekGridOffset = 1;
                string dayOfWeekOfAppointment = validAppointment.Date.DayOfWeek.ToString();
                int xCoordinateToInsert = GetXCoordinateOfDay(validAppointment.Date);

                double startHourInMinutes = (validAppointment.StartTime - validAppointment.StartTime.Date).TotalMinutes;
                double endHourInMinutes = (validAppointment.EndTime - validAppointment.EndTime.Date).TotalMinutes;
                double rectangleHeight = endHourInMinutes - startHourInMinutes;
                double topMargin = startHourInMinutes;
                double bottomMargin = minutesInADay - endHourInMinutes;
                Rectangle rectangle = GenerateRectangleForAppointment(randomizer, rectangleHeight, topMargin, bottomMargin, xCoordinateToInsert, weekGridOffset);
                gridDayHoursNumbers.Children.Add(rectangle);
                AddLabelsToWeekAppointment(topMargin, bottomMargin, validAppointment, xCoordinateToInsert, weekGridOffset);
            }
        }
        private void AddLabelsToWeekAppointment(double topMargin, double bottomMargin, Appointment validAppointment, int xCoordinateToInsert, int weekGridOffset)
        {
            Label titleLabel = GenerateAppointmentTitleLabel(topMargin, bottomMargin, validAppointment);
            Grid.SetColumn(titleLabel, xCoordinateToInsert + weekGridOffset);
            Grid.SetRowSpan(titleLabel, hoursInADay);
            gridDayHoursNumbers.Children.Add(titleLabel);

            int titleTextLines = titleLabel.Content.ToString().Split('\n').Length;
            Label descriptionLabel = GenerateAppointDescriptionLabel(topMargin, titleTextLines, bottomMargin, validAppointment);
            Grid.SetColumn(descriptionLabel, xCoordinateToInsert + weekGridOffset);
            Grid.SetRowSpan(descriptionLabel, hoursInADay);
            gridDayHoursNumbers.Children.Add(descriptionLabel);

        }
        private static void EditButton_Click(Appointment appointment, MainWindow mainWindow)
        {
            mainWindow.targetAppoitnemnt = appointment;
            mainWindow.DisplayEditAppointmentMenu();
            mainWindow.UpdateEditAppointmentMenu();
        }        
        private static Color GenerateRandomColor(Random randomizer)
        {
            Color randomColor = Color.FromRgb((byte)randomizer.Next(0, 255), (byte)randomizer.Next(0, 255), (byte)randomizer.Next(0, 255));
            return randomColor;
        }        
        private bool CheckifAppointmentIsValid()
        {
            bool isAppointmentTitleValid = false;
            bool isAppointmentDescriptionValid = false;
            bool isAppointmentDateValid = false;
            bool isAppointmentStartHourValid = false;
            bool isAppointmentEndHourValid = false;

            if (txtTitleNewAppointment.Text.Trim(' ').Length > 0)
            {
                isAppointmentTitleValid = true;
            }
            if (txtDecriptionNewAppointment.Text.Trim(' ').Length > 0)
            {
                isAppointmentDescriptionValid = true;
            }
            if (datePickerNewAppointment.SelectedDate.HasValue)
            {
                isAppointmentDateValid = true;
            }
            if (timePickerStartTimeNewAppointment.Value != null)
            {
                isAppointmentStartHourValid = true;
            }

            bool isTimePickerNotNull = timePickerEndTimeNewAppointment.Value != null;
            bool isTimePickerEndTimeAfterStartTime = timePickerStartTimeNewAppointment.Value < timePickerEndTimeNewAppointment.Value;

            if (isTimePickerNotNull && isTimePickerEndTimeAfterStartTime)
            {
                isAppointmentEndHourValid = true;
            }

            bool isAppointmentCompletelyValid = isAppointmentTitleValid && isAppointmentDescriptionValid &&
                isAppointmentDateValid && isAppointmentStartHourValid && isAppointmentEndHourValid;
            if (isAppointmentCompletelyValid)
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }
        private void ClearWeekGrids()
        {
            gridDayHoursNumbers.Children.Clear();
            gridWeeklyDays.Children.Clear();
        }
        private void DisplayMonthMenu()
        {
            gridEnterUser.Visibility = Visibility.Hidden;
            gridMonthlyView.Visibility = Visibility.Visible;
            gridWeeklyView.Visibility = Visibility.Hidden;
            gridAppointmentCreation.Visibility = Visibility.Hidden;
            gridAppointmentListing.Visibility = Visibility.Hidden;
            isArrowInputAvailable = true;
        }        
        private void DisplayWeeklyViewMenu()
        {
            gridEnterUser.Visibility = Visibility.Hidden;
            gridMonthlyView.Visibility = Visibility.Hidden;
            gridWeeklyView.Visibility = Visibility.Visible;
            gridAppointmentCreation.Visibility = Visibility.Hidden;
            gridAppointmentListing.Visibility = Visibility.Hidden;
            isArrowInputAvailable = false;
        }
        private void DisplayAppointmentsList()
        {
            gridEnterUser.Visibility = Visibility.Hidden;
            gridMonthlyView.Visibility = Visibility.Hidden;
            gridWeeklyView.Visibility = Visibility.Hidden;
            gridAppointmentCreation.Visibility = Visibility.Hidden;
            gridAppointmentListing.Visibility = Visibility.Visible;
            isArrowInputAvailable = false;
        }
        private void AddAppointmentToEachDay(int xCoordinateToInsertNumber, int yCoordinateToInsertNumber, DateTime firstDayOfMonth, int numberOfDay)
        {
            DateTime dayToCheckAppointments = firstDayOfMonth.Date.AddDays(numberOfDay - 1); //The -1 is given so that we count from theorical day 0 insted of 1.
            List<Appointment> dayAppointments = SelectDayAppointments(dayToCheckAppointments);
            if (dayAppointments.Count != 0)
            {
                Label appointmentLabel = new Label()
                {
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 15,
                    Content = ""
                };
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Appointment appointment in dayAppointments)
                {
                    if (appointmentLabel.Content.ToString().Length > 0)
                    {
                        stringBuilder.Append(appointment.Title);
                    }
                    else
                    {

                        stringBuilder.Append("\n");
                        stringBuilder.Append(appointment.Title);
                    }
                    stringBuilder.Append("\n");
                    appointmentLabel.Content = stringBuilder;
                }
                Grid.SetColumn(appointmentLabel, xCoordinateToInsertNumber);
                Grid.SetRow(appointmentLabel, yCoordinateToInsertNumber);
                gridCalendar.Children.Add(appointmentLabel);
            }
        }
        private void AddLabelNumbersToEachDay(int xCoordinateToInsertNumber, int yCoordinateToInsertNumber, int numberOfDay)
        {
            Label label = new Label()
            {
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 25,
                Content = numberOfDay,
            };
            Grid.SetColumn(label, xCoordinateToInsertNumber);
            Grid.SetRow(label, yCoordinateToInsertNumber);
            gridCalendar.Children.Add(label);
        }
        private void AddDifferentColourBackgroundsToCalendar(int xCoordinateToInsertNumber, int yCoordinateToInsertNumber)
        {
            bool isXCoordinateSaurday = xCoordinateToInsertNumber != saturdayPosition;
            bool isXCoordinateSunday = xCoordinateToInsertNumber != sundayPosition;
            bool isXCoordinateWeekend = isXCoordinateSaurday && isXCoordinateSunday;

            if (isXCoordinateWeekend)
            {
                Rectangle rectangleDaysBackground = new Rectangle()
                {
                    Fill = Brushes.White
                };
                Grid.SetColumn(rectangleDaysBackground, xCoordinateToInsertNumber);
                Grid.SetRow(rectangleDaysBackground, yCoordinateToInsertNumber);
                gridCalendar.Children.Add(rectangleDaysBackground);
            }
            else
            {
                Rectangle rectangleWeekendBackground = new Rectangle()
                {
                    Fill = (new BrushConverter().ConvertFrom("#c1f7da")) as SolidColorBrush,
                };
                Grid.SetColumn(rectangleWeekendBackground, xCoordinateToInsertNumber);
                Grid.SetRow(rectangleWeekendBackground, yCoordinateToInsertNumber);
                gridCalendar.Children.Add(rectangleWeekendBackground);
            }
        }
        private void FillGridWeeklyDaysLabels(DateTime dateOfReference)
        {
            DateTime mondayDate = GetFirstDayOfWeek(dateOfReference);
            const int numberDaysOfWeek = 7;
            for (int xCoordinateOfWeek = 0; xCoordinateOfWeek < numberDaysOfWeek; xCoordinateOfWeek++)
            {
                StringBuilder labelContent = new StringBuilder();
                switch (xCoordinateOfWeek)
                {
                    case mondayPosition:
                        labelContent.Append("MON ");
                        break;
                    case tuesdayPosition:
                        labelContent.Append("TUE ");
                        break;
                    case wednesdayPosition:
                        labelContent.Append("WED ");
                        break;
                    case thursdayPosition:
                        labelContent.Append("THU ");
                        break;
                    case fridayPosition:
                        labelContent.Append("FRI ");
                        break;
                    case saturdayPosition:
                        labelContent.Append("SAT ");
                        break;
                    case sundayPosition:
                        labelContent.Append("SUN ");
                        break;
                }
                labelContent.Append(mondayDate.AddDays(xCoordinateOfWeek).Day.ToString(CultureInfo.CurrentCulture));
                Label dayLabel = new Label
                {
                    Content = labelContent,
                    Foreground = (new BrushConverter().ConvertFrom("#999999")) as SolidColorBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 35,
                };
                int xCoordinateColumnOffset = 1;
                Grid.SetColumn(dayLabel, xCoordinateOfWeek + xCoordinateColumnOffset);
                gridWeeklyDays.Children.Add(dayLabel);
            }
        }        
        private void SaveAppointment()
        {
            string newAppointmentTitle = txtTitleNewAppointment.Text;
            string newAppointmentDescription = txtDecriptionNewAppointment.Text;
            DateTime newAppointmentDate = datePickerNewAppointment.SelectedDate.Value.Date;
            DateTime newAppointmentStartTime = timePickerStartTimeNewAppointment.Value.Value;
            DateTime newAppointmentEndTime = timePickerEndTimeNewAppointment.Value.Value;
            List<User> invitedUsers = new List<User>();
            foreach (string userName in listboxAppointmentInvitations.SelectedItems)
            {
                User invitedUser = users.Find(u => u.Name == userName);
                invitedUsers.Add(invitedUser);
            }
            bool isAppointmentNew = (targetAppoitnemnt.Title == null);
            if (isAppointmentNew)
            {
                Appointment appointment = new Appointment(newAppointmentTitle, newAppointmentDescription,
                    newAppointmentDate, newAppointmentStartTime, newAppointmentEndTime, currentUser, invitedUsers);
                appointments.Add(appointment);
            }
            else
            {
                int indexOfAppointmentToChange = appointments.IndexOf(targetAppoitnemnt);
                appointments[indexOfAppointmentToChange] = new Appointment(newAppointmentTitle, newAppointmentDescription,
                    newAppointmentDate, newAppointmentStartTime, newAppointmentEndTime, currentUser, invitedUsers);
            }
        }                
        private void UpdateWeekMonthLabel(DateTime dateOfReference)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            string monthName = GetFirstDayOfWeek(dateOfReference).ToString("MMMM", usEnglish).ToUpper(CultureInfo.CurrentCulture);
            StringBuilder contentForLabel = new StringBuilder();
            contentForLabel.Append(monthName);
            int daysToLastDayOfWeek = 6;
            string weekendMonthName = GetFirstDayOfWeek(dateOfReference).AddDays(daysToLastDayOfWeek).ToString("MMMM", usEnglish).ToUpper(CultureInfo.CurrentCulture);
            if (monthName != weekendMonthName)
            {
                contentForLabel.Append(" / ");
                contentForLabel.Append(weekendMonthName);
            }
            lblWeekMonth.Content = contentForLabel;
        }        
        private void BtnNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            targetAppoitnemnt = new Appointment();
            if (gridMonthlyView.Visibility == Visibility.Visible)
            {
                gridToReturnFromAppointment = GridsToReturnFromAppointment.MonthlyView;
            }
            else if (gridWeeklyView.Visibility == Visibility.Visible)
            {
                gridToReturnFromAppointment = GridsToReturnFromAppointment.WeeklyView;
            }
            UpdateEditAppointmentMenu();
            DisplayEditAppointmentMenu();
        }
        private void UpdateEditAppointmentMenu()
        {
            txtTitleNewAppointment.Text = targetAppoitnemnt.Title;
            txtDecriptionNewAppointment.Text = targetAppoitnemnt.Description;
            if (targetAppoitnemnt.Date == DateTime.MinValue)
            {
                datePickerNewAppointment.SelectedDate = DateTime.Now;
            }
            else
            {
                datePickerNewAppointment.SelectedDate = targetAppoitnemnt.Date;
            }
            timePickerStartTimeNewAppointment.Value = targetAppoitnemnt.StartTime;
            timePickerEndTimeNewAppointment.Value = targetAppoitnemnt.EndTime;
            listboxAppointmentInvitations.Items.Clear();
            int currentIndex = 0;
            foreach (User user in users)
            {
                bool isUserCreatingAppointment = user == currentUser;
                bool isUserInvited = targetAppoitnemnt.IsUserInvited(user);
                if (!isUserCreatingAppointment)
                {
                    listboxAppointmentInvitations.Items.Add(user.Name);
                    if (isUserInvited)
                    {
                        listboxAppointmentInvitations.SelectedItems.Add(user.Name);
                    }
                    currentIndex++;
                }
            }
        }
        
        private void DisplayEditAppointmentMenu()
        {
            gridEnterUser.Visibility = Visibility.Hidden;
            gridMonthlyView.Visibility = Visibility.Hidden;
            gridWeeklyView.Visibility = Visibility.Hidden;
            gridAppointmentCreation.Visibility = Visibility.Visible;
            gridAppointmentListing.Visibility = Visibility.Hidden;
            isArrowInputAvailable = false;
        }
        private List<Appointment> SelectDayAppointments(DateTime day)
        {
            DateTime dayHourZero = day.Date;
            DateTime lastSecondOfDay = dayHourZero.AddDays(1).AddMilliseconds(-1);
            List<Appointment> validAppointments = appointments.FindAll(a => a.IsBetweenDates(dayHourZero, lastSecondOfDay));
            List<Appointment> validUserAppointments = validAppointments.FindAll(a => a.IsUserInvited(currentUser));
            return validUserAppointments;

        }
        #endregion
    }
}
