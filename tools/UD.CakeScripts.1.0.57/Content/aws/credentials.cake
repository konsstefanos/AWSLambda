using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

void EnsureCredentials()
{
    if (BuildEngine.RemoteAwsCredentials != null) return;

    var credentialStore = new CredentialProfileStoreChain();
    if (!credentialStore.TryGetProfile(BuildEngine.Profile, out var profile))
        throw new Exception($"Could not find AWS Profile, no profile named '{BuildEngine.Profile}'");
    BuildEngine.SetRemoteCredentials(AWSCredentialsFactory.GetAWSCredentials(profile, credentialStore));
    var assumeRoleCredentials = BuildEngine.RemoteAwsCredentials as AssumeRoleAWSCredentials;
    if (assumeRoleCredentials != null)
        assumeRoleCredentials.Options.MfaTokenCodeCallback = () => {
            Console.Write($"Enter MFA code for '{BuildEngine.Profile}': ");
            var code = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    code += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && code.Length > 0)
                    {
                        code = code.Substring(0, (code.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return code;
        };
}

void EnsureAwsProfile()
{
    var profileName = "localstack";
    CredentialProfile awsProfile;
    var credentialStore = new SharedCredentialsFile();
    if (!credentialStore.TryGetProfile(profileName, out awsProfile))
    {
        var options = new CredentialProfileOptions
        {
            AccessKey = "localstack",
            SecretKey = "localstack"
        };
        awsProfile = new CredentialProfile(profileName, options) {Region = RegionEndpoint.EUWest1};
        credentialStore.RegisterProfile(awsProfile);
        Information($"Profile: '{profileName}' has been created");
    }
    BuildEngine.SetLocalCredentials(awsProfile.GetAWSCredentials(credentialStore));
}
