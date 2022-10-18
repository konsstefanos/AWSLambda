Task("Update-IAM")
    .Description("Updates the IAM Roles for a specified Account")
    .Does(async () =>
    {
        var client = GetCloudFormationClient();
        var response = await client.UpdateStackAsync(new UpdateStackRequest
        {
            Capabilities = new List<string> { "CAPABILITY_IAM" },
            StackName = $"{BuildEngine.StackName}-iam",
            TemplateBody = System.IO.File.ReadAllText("./template_iam.yaml")
        });
        Information(la => la("IAM update result {0}, StackId: {1}", response.HttpStatusCode, response.StackId));
    });
