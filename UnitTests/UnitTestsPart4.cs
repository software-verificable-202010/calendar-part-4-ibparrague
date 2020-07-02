using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proyecto_2_Software_Verificable;
using Proyecto4SoftwareVerificable;

namespace UnitTests
{
    [TestClass]
    public class UnitTestsPart4
    {
        [TestMethod]
        public void GetFirstDayOfWeek_ValidThurdsdayDate_ValidMondayDate()
        {
            DateTime thursdayFourthOfJune = new DateTime(2020, 06, 04);
            DateTime mondayFirstOfJune = new DateTime(2020, 06, 01);

            DateTime testedMethodResult = MainWindow.GetFirstDayOfWeek(thursdayFourthOfJune);
            Assert.AreEqual(mondayFirstOfJune, testedMethodResult);
        }
        [TestMethod]
        public void GetFirstDayOfWeek_ValidFridayDate_ValidMondayDate()
        {
            DateTime fridayFifthOfJune = new DateTime(2020, 06, 05);
            DateTime mondayFirstOfJune = new DateTime(2020, 06, 01);

            DateTime testedMethodResult = MainWindow.GetFirstDayOfWeek(fridayFifthOfJune);
            Assert.AreEqual(mondayFirstOfJune, testedMethodResult);
        }

        [TestMethod]
        public void IsBetweenDates_ValidAppointment_ReturnsTrue()
        {
            string appointmentTitle = "test";
            string appointmentDescription = "UnitTest Appointment";
            DateTime appointmentDate = new DateTime(2020, 06, 04);
            DateTime appointmentStartTime = new DateTime(01, 01, 01, 10, 0, 00);
            DateTime appointmentEndTime = new DateTime(01, 01, 01, 15, 0, 00);
            User user = new User();
            List<User> invitedUsers = new List<User>();
            Appointment appointment = new Appointment(appointmentTitle, appointmentDescription, appointmentDate, appointmentStartTime, appointmentEndTime, user, invitedUsers);
            DateTime firstDate = new DateTime(2020, 06, 01);
            DateTime secondDate = new DateTime(2020, 06, 07);
            bool isBetweenDates = appointment.IsBetweenDates(firstDate, secondDate);
            Assert.IsTrue(isBetweenDates);
        }

        [TestMethod]
        public void IsUserInvited_ValidAppointmentAndUser_ReturnsTrue()
        {
            string appointmentTitle = "test";
            string appointmentDescription = "UnitTest Appointment";
            DateTime appointmentDate = new DateTime(2020, 06, 04);
            DateTime appointmentStartTime = new DateTime(01, 01, 01, 10, 0, 00);
            DateTime appointmentEndTime = new DateTime(01, 01, 01, 15, 0, 00);

            string userName = "TestUser";
            User testUser = new User(userName);
            List<User> invitedUsers = new List<User>();
            invitedUsers.Add(testUser);
            Appointment appointment = new Appointment(appointmentTitle, appointmentDescription, appointmentDate, appointmentStartTime, appointmentEndTime, testUser, invitedUsers);
            bool isUserInvited = appointment.IsUserInvited(testUser);
            Assert.IsTrue(isUserInvited);
        }

        [TestMethod]
        public void SerializeAppointments_AppointmentsToSerialize_AppointmentFileCreated()
        {
            string serializedAppointmentsFileName = "TestSerialzedAppointments.txt";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(serializedAppointmentsFileName);
            string appointmentsFilePath = stringBuilder.ToString();

            if (File.Exists(appointmentsFilePath))
            {
                File.Delete(appointmentsFilePath);
            }

            string testAppointmenttitle = "Test Title";
            string testAppointmentdescription = "test description";
            DateTime testAppointmentdate = new DateTime(2020, 07, 01);
            DateTime testAppointmentstartTime = new DateTime(1, 1, 1, 12, 00, 00);
            DateTime testAppointmentendTime = new DateTime(1, 1, 1, 12, 00, 00);
            User testAppointmentcreator = new User("TestUser");
            User testInvitedUser = new User("TestUserFriend");
            List<User> testAppointmentInvitedUsers = new List<User>();
            testAppointmentInvitedUsers.Add(testInvitedUser);
            Appointment testAppointment = new Appointment(testAppointmenttitle, testAppointmentdescription, testAppointmentdate,
                testAppointmentstartTime, testAppointmentendTime, testAppointmentcreator, testAppointmentInvitedUsers);

            List<Appointment> appointments = new List<Appointment>();
            appointments.Add(testAppointment);

            
            MainWindow.SerializeAppointments(appointments, serializedAppointmentsFileName);

            bool fileWasCreated = File.Exists(appointmentsFilePath);
            Assert.IsTrue(fileWasCreated);
        }
        
        [TestMethod]
        public void DeserializeAppointments_FileWithAppointmentToDeserialize_AppointmentDeserializedIsSameAsTest()
        {
            string serializedAppointmentsFileName = "SerializedAppointmentsToDeserialize.txt";
            string currentFolder = Environment.CurrentDirectory;
            string testFileLocation = String.Format("{0}\\{1}", currentFolder, serializedAppointmentsFileName);
            if (!File.Exists(testFileLocation))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(currentFolder);
                DirectoryInfo parentDirectory = directoryInfo.Parent;
                DirectoryInfo projectDirectory = parentDirectory.Parent;
                string projectDirectoryName = projectDirectory.FullName;
                string testFilePath = String.Format("{0}\\{1}", projectDirectoryName, serializedAppointmentsFileName);
                File.Copy(testFilePath, testFileLocation);
            }
            string testAppointmenttitle = "Test Title";
            string testAppointmentdescription = "test description";
            DateTime testAppointmentdate = new DateTime(2020, 07, 01);
            DateTime testAppointmentstartTime = new DateTime(1, 1, 1, 12, 00, 00);
            DateTime testAppointmentendTime = new DateTime(1, 1, 1, 12, 00, 00);
            User testAppointmentcreator = new User("TestUser");
            User testInvitedUser = new User("TestUserFriend");
            List<User> testAppointmentInvitedUsers = new List<User>();
            testAppointmentInvitedUsers.Add(testInvitedUser);
            Appointment testAppointment = new Appointment(testAppointmenttitle, testAppointmentdescription, testAppointmentdate,
                testAppointmentstartTime, testAppointmentendTime, testAppointmentcreator, testAppointmentInvitedUsers);

            List<Appointment> appointment = new List<Appointment>();

            appointment = MainWindow.DeserializeAppointments(serializedAppointmentsFileName);

            Appointment deserializedAppointment = appointment[0];

            bool isTitleSame = (deserializedAppointment.Title == testAppointment.Title);
            bool isDescriptionSame = (deserializedAppointment.Description == testAppointment.Description);
            bool isDateSame = (deserializedAppointment.Date == testAppointment.Date);
            bool isStartSame = (deserializedAppointment.StartTime == testAppointment.StartTime);
            bool isEndSame = (deserializedAppointment.EndTime == testAppointment.EndTime);

            bool isResultOk = isTitleSame && isDateSame && isDateSame && isStartSame && isEndSame;
            Assert.IsTrue(isResultOk);
        }

        [TestMethod]
        public void SerializeUsers_AppointmentsToSerialize_AppointmentFileCreated()
        {
            string serializedUsersFileName = "TestSerialzedUsers.txt";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.CurrentDirectory);
            stringBuilder.Append("\\");
            stringBuilder.Append(serializedUsersFileName);
            string usersFilePath = stringBuilder.ToString();

            if (File.Exists(usersFilePath))
            {
                File.Delete(usersFilePath);
            }

            User testUser = new User("TestUser");
            List<User> users = new List<User>();
            users.Add(testUser);

            MainWindow.SerializeUsers(users, serializedUsersFileName);

            bool fileWasCreated = File.Exists(usersFilePath);
            Assert.IsTrue(fileWasCreated);
        }

        [TestMethod]
        public void DeserializeUsers_FileWithAppointmentToDeserialize_AppointmentDeserializedIsSameAsTest()
        {
            string serializedUsersFileName = "SerializedUsersToDeserialize.txt";
            string currentFolder = Environment.CurrentDirectory;
            string testFileLocation = String.Format("{0}\\{1}", currentFolder, serializedUsersFileName);
            if (!File.Exists(testFileLocation))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(currentFolder);
                DirectoryInfo parentDirectory = directoryInfo.Parent;
                DirectoryInfo projectDirectory = parentDirectory.Parent;
                string projectDirectoryName = projectDirectory.FullName;
                string testFilePath = String.Format("{0}\\{1}", projectDirectoryName, serializedUsersFileName);
                File.Copy(testFilePath, testFileLocation);
            }

            User testUser = new User("TestUser");
            List<User> user = new List<User>();
            user = MainWindow.DeserializeUsers(serializedUsersFileName);
            User deserializedUser = user[0];

            bool isNameSame = (deserializedUser.Name == testUser.Name);

            //Users only have a name, so if that name is same, users are the same.
            Assert.IsTrue(isNameSame);
        }

        [TestMethod]
        public void CustomRectangle_CustomRectangleStyleBlue_RectangleComesAsExpected()
        {
            var blueRectangleType = MainWindow.CustomRectangleStyle.blueLines;
            Rectangle testRectangle = MainWindow.CustomRectangle(blueRectangleType);

            var expectedBrush = Brushes.Transparent;

            var blueStroke = (new BrushConverter().ConvertFrom("#1f3861"));
            SolidColorBrush expectedStroke = blueStroke as SolidColorBrush;
            double expectedStrokeThickness = 2;

            bool isBrushCorrect = testRectangle.Fill == expectedBrush;
            bool isStrokeCorrect = testRectangle.Stroke.ToString() == expectedStroke.ToString();
            bool isStrokeThicknessCorrect = testRectangle.StrokeThickness == expectedStrokeThickness;

            bool isResultOkay = isBrushCorrect && isStrokeCorrect && isStrokeThicknessCorrect;

            Assert.IsTrue(isResultOkay);
        }

        [TestMethod]
        public void CustomRectangle_CustomRectangleStyleGray_RectangleComesAsExpected()
        {
            var grayRectangleType = MainWindow.CustomRectangleStyle.grayLines;
            Rectangle testRectangle = MainWindow.CustomRectangle(grayRectangleType);

            var expectedBrush = Brushes.Transparent;

            var grayStroke = (new BrushConverter().ConvertFrom("#999999"));
            SolidColorBrush expectedStroke = grayStroke as SolidColorBrush;
            double expectedStrokeThickness = 0.5;

            bool isBrushCorrect = testRectangle.Fill == expectedBrush;
            bool isStrokeCorrect = testRectangle.Stroke.ToString() == expectedStroke.ToString();
            bool isStrokeThicknessCorrect = testRectangle.StrokeThickness == expectedStrokeThickness;

            bool isResultOkay = isBrushCorrect && isStrokeCorrect && isStrokeThicknessCorrect;

            Assert.IsTrue(isResultOkay);
        }

        [TestMethod]
        public void GenerateAppointmentTitleLabel_TopMarginBottomMarginAndAppointment_LabelComesAsExpected()
        {
            int topMargin = 2;
            int bottomMargin = 2;
            Appointment appointment = new Appointment();
            string appoitnmentTitle = "test";
            appointment.Title = appoitnmentTitle;
            Label testLabel = MainWindow.GenerateAppointmentTitleLabel(topMargin, bottomMargin, appointment);

            string expectedTestLabelText = "test";
            Thickness expectedMargin = new Thickness(0, topMargin, 0, bottomMargin);
            int expectedFontSize = 20;

            string testLabelContent = testLabel.Content.ToString();

            bool isTextOk = testLabelContent == expectedTestLabelText;
            bool isMarginOk = testLabel.Margin == expectedMargin;
            bool isFontSizeOk = testLabel.FontSize == expectedFontSize;
            bool isResultsOk = isTextOk && isMarginOk && isFontSizeOk;

            Assert.IsTrue(isResultsOk);
        }

        [TestMethod]
        public void GetXCoordinateOfDay_WednesdayEightJuly_CoordinateTwo()
        {
            int expectedPosition = 2;
            DateTime wendesdayEightOfJuly = new DateTime(2020, 07, 08);

            int testPosition = MainWindow.GetXCoordinateOfDay(wendesdayEightOfJuly);

            Assert.AreEqual(expectedPosition, testPosition);
        }

        [TestMethod]
        public void GetXCoordinateOfDay_ThursdayNineJuly_CoordinateThree()
        {
            int expectedPosition = 3;
            DateTime thursdayNineOfJuly = new DateTime(2020, 07, 09);

            int testPosition = MainWindow.GetXCoordinateOfDay(thursdayNineOfJuly);

            Assert.AreEqual(expectedPosition, testPosition);
        }

        [TestMethod]
        public void GenerateAppointDescriptionLabel_TopMarginTitleTextLinesBottomMarginAndValidAppointment_LabelComesAsExpected()
        {
            Appointment appointment = new Appointment();
            string appointmentTitle = "test";
            string appointmentDescription = "testdescription";
            appointment.Title = appointmentTitle;
            appointment.Description = appointmentDescription;

            int topMargin = 1;
            int titleTextLines = 1;
            int bottomMargin = 1;
            int titleFontLinePixelsSize = 25;

            Label testLabel = MainWindow.GenerateAppointDescriptionLabel(topMargin, titleTextLines, bottomMargin, appointment);

            string expectedTestLabelText = "testdescription\n";
            int expectedFontSize = 10;
            Thickness expectedMargin = new Thickness(0, topMargin + (titleFontLinePixelsSize * titleTextLines), 0, bottomMargin);

            string testLabelContent = testLabel.Content.ToString();


            bool isTextOk = testLabelContent == expectedTestLabelText;
            bool isMarginOk = testLabel.Margin == expectedMargin;
            bool isFontSizeOk = testLabel.FontSize == expectedFontSize;
            bool isResultsOk = isTextOk && isMarginOk && isFontSizeOk;

            Assert.IsTrue(isResultsOk);
        }

        [TestMethod]
        public void GenerateRectangleForAppointment_AnyArgsAskedByMethod_RectangleComesAsExpected()
        {
            Random randomizer = new Random();
            double topMargin = 1;
            double bottomMargin = 1;
            double rectangleHeight = 1;
            int xCoordinate = 1;
            int weekOffset = 0;

            Rectangle testRectangle = MainWindow.GenerateRectangleForAppointment(randomizer, rectangleHeight, topMargin, bottomMargin, xCoordinate, weekOffset);

            double expectedHeight = 1;
            Thickness expectedMargin = new Thickness(0, topMargin, 0, bottomMargin);
            Brush expectedFill = Brushes.Transparent;
            int expectedStrokeThickness = 5;
            //Colour not tested, as it is random.

            bool isHeightOk = testRectangle.Height == expectedHeight;
            bool isMarginOk = testRectangle.Margin == expectedMargin;
            bool isFillOk = testRectangle.Fill == expectedFill;
            bool isStrokeOk = testRectangle.StrokeThickness == expectedStrokeThickness;

            bool isResultOk = isHeightOk && isMarginOk && isFillOk && isStrokeOk;

            Assert.IsTrue(isResultOk);

        }

        [TestMethod]
        public void AddColumnsToAppointmentGrid_EmptyGrid_GridWithFiveColumnDefinitions()
        {
            Grid testGrid = new Grid();

            MainWindow.AddColumnsToAppointmentGrid(testGrid);

            int columnAmount = testGrid.ColumnDefinitions.Count;
            int expectedColumnAmount = 5;

            Assert.AreEqual(expectedColumnAmount, columnAmount);
        }

        [TestMethod]
        public void AddLabelsToAppointmentGrid_EmptyGridAndValidAppointment_GridWithThreeLabels()
        {
            Grid testGrid = new Grid();

            string testAppointmentTitle = "test";
            Appointment testAppointment = new Appointment();
            testAppointment.Title = testAppointmentTitle;

            MainWindow.AddLabelsToAppointmentGrid(testAppointment, testGrid);

            int childrenExpected = 3;
            int testGridChildren = testGrid.Children.Count;

            Assert.AreEqual(childrenExpected, testGridChildren);
        }


    }
}