using System.Diagnostics;

namespace AribaEats.Tests
{
    public class AribaEatsUnitTests
    {

        [Fact]
        public async Task Test_01_StartupMenu()
        {
            await RunTestForFileAsync("01. Startup Menu");
        }

        [Fact]
        public async Task Test_02_RegisterCustomer()
        {
            await RunTestForFileAsync("02. Register Customer");
        }

        [Fact]
        public async Task Test_03_Register_Customer_InvalidName()
        {
            await RunTestForFileAsync("03. Register Customer - Invalid Name");
        }

        [Fact]
        public async Task Test_04_Register_Customer_InvalidAge()
        {
            await RunTestForFileAsync("04. Register Customer - Invalid Age");
        }

        [Fact]
        public async Task Test_05_Register_Customer_InvalidEmail()
        {
            await RunTestForFileAsync("05. Register Customer - Invalid Email");
        }
        
        [Fact]
        public async Task Test_06_Register_Customer_InvalidPhoneNumber()
        {
            await RunTestForFileAsync("06. Register Customer - Invalid Mobile Number");
        }

        [Fact]
        public async Task Test_07_Register_Customer_InvalidPassword()
        {
            await RunTestForFileAsync("07. Register Customer - Invalid Password");
        }

        [Fact]
        public async Task Test_08_Register_Customer_InvalidLocation()
        {
            await RunTestForFileAsync("08. Register Customer - Invalid Location");
        }

        [Fact]
        public async Task Test_09_Register_Customer_EmailAlreadyExits()
        {
           await RunTestForFileAsync("09. Register Customer - Email Already In Use");
        }

        [Fact]
        public async Task Test_10_LoginAsCustomer_ShouldBeSuccessful()
        {
            await RunTestForFileAsync("10. Login as Customer");
        }

        [Fact]
        public async Task Test_11_LoginAsCustomer_WrongEmailOrPassword_ShouldBeUnsuccessful()
        {
            await RunTestForFileAsync("11. Login as Customer - Wrong Email or Password");
        }

        [Fact]
        public async Task Test_12_DisplayCustomerInformation()
        {
            await RunTestForFileAsync("12. Display User Information - Customer");
        }

        [Fact]
        public async Task Test_13_LoginAsCustomer_TryEachOption()
        {
            await RunTestForFileAsync("13. Login as Customer, try each option");
        }

        [Fact]
        public async Task Test_14_DisplayUserInformation_MultipleCustomers()
        {
            await RunTestForFileAsync("14. Display User Information - Multiple Customers");
        }

        [Fact]
        public async Task Test_15_RegisterDeliverer()
        {
            await RunTestForFileAsync("15. Register Deliverer and Login");
        }

        [Fact]
        public async Task Test_16_RegisterDeliverer_InvalidPlate()
        {
            await RunTestForFileAsync("16. Register Deliverer - Invalid Plate");
        }

        [Fact]
        public async Task Test_17_DisplayUserInformation_DelivererInfo()
        {
            await RunTestForFileAsync("17. Display User Information - Deliverer");
        }

        [Fact]
        public async Task Test_18_RegisterClientAndLogin()
        {
            await RunTestForFileAsync("18. Register Client and Login");
        }

        [Fact]
        public async Task Test_19_RegisterClient_InvalidRestaurantName()
        {
            await RunTestForFileAsync("19. Register Client - Invalid Restaurant Name");
        }

        [Fact]
        public async Task Test_20_DisplayUserInformation_Client()
        {
            await RunTestForFileAsync("20. Display User Information - Client");
        }

        private async Task RunTestForFileAsync(string fileName)
        {
            // Arrange
            string input = File.ReadAllText(Path.Combine("TestData", "Inputs", fileName + ".txt"));
            string expected = File.ReadAllText(Path.Combine("TestData", "RefOutputs", fileName + ".txt"));

            // Sanitize input (if needed)
            input = SanitizeString(input);

            // Act
            // Run the program with the input
            string actual = await RunMainProgramWithInput(input);

            // Sanitize output for comparison
            expected = SanitizeString(expected);
            actual = SanitizeString(actual);

            // Assert
            Assert.Equal(expected, actual);
        }

        private async Task<string> RunMainProgramWithInput(string input)
        {
            // Path to your application executable
            string exePath = Path.Combine("..", "..", "..", "..", "AribaEats", "bin", "Debug", "net6.0", "AribaEats.exe");

            // Create process start info
            var processStartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,  // Capture error output as well
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string output;
            // Start the process with using statement to ensure disposal
            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start the process.");
                }

                // Setup for asynchronous read of standard output
                var outputTask = Task.Run(() => process.StandardOutput.ReadToEnd());
                var errorTask = Task.Run(() => process.StandardError.ReadToEnd());

                // Write input to the process
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close(); // Closing the input stream

                // Wait for process to exit with timeout
                if (!process.WaitForExit(10000)) // 10-second timeout
                {
                    // Force termination if it doesn't exit on its own
                    process.Kill();
                }

                // Read output and error asynchronously
                output = await outputTask; // Get the standard output
                string errorOutput = await errorTask; // Get the error output

                // Handle errors if needed
                if (!string.IsNullOrEmpty(errorOutput))
                {
                    // Log or handle the error output as needed
                    Console.WriteLine($"Error Output: {errorOutput}");
                }
            } // The process is disposed here, releasing the file lock

            return output;
        }


        private string SanitizeString(string text)
        {
            // Remove carriage returns and normalize line endings to just \n
            string sanitized = text.Replace("\r\n", "\n").Replace("\r", "\n");
            sanitized = sanitized.Trim();

            return sanitized;
        }
        
    }

}