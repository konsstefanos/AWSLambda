Task("Update-PreReqs")
    .Description("Updates the Pre-Requisites stack for a specified Account")
    .Does(async () =>
    {
        var client = GetCloudFormationClient();
        var response = await client.UpdateStackAsync(new UpdateStackRequest
        {
            Capabilities = new List<string> { "CAPABILITY_IAM" },
            StackName = $"{BuildEngine.StackName}-prereqs",
            TemplateBody = System.IO.File.ReadAllText("./template_prereqs.yaml")
        });
        Information(la => la("Pre-Reqs update result {0}, StackId: {1}", response.HttpStatusCode, response.StackId));
    });
