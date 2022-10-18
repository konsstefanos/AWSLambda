Task("Bootstrap-Stack")
    .Description("Creates empty stacks so that change sets can be generated")
    .Does(async () =>
    {
        string stack;
        string roleArn = null;
        var capabilities = new List<string> { "CAPABILITY_IAM" };
        var template = @"AWSTemplateFormatVersion: ""2010-09-09""
Description: Initial Bootstrap
Parameters:
  CreateResources:
    Type: Number
    AllowedValues: [0]
    Default: 0
Conditions:
  ShouldCreate: !Equals [!Ref CreateResources, 1]
Resources:
  StackParameter:
    Type: AWS::SSM::Parameter
    Condition: ShouldCreate
    Properties:
      Type: String
      Name: /WillNotCreate
      Value: missing
";
        var client = GetCloudFormationClient();
        if (string.IsNullOrWhiteSpace(BuildEngine.StackType))
        {
            stack = BuildEngine.StackName;
            StackResource resource;
            try
            {
                var response = await client.DescribeStackResourcesAsync(new DescribeStackResourcesRequest
                {
                    StackName = $"{BuildEngine.StackName}-iam"
                });
                resource = response.StackResources
                    .FirstOrDefault(r =>
                        r.ResourceType == "AWS::IAM::Role" &&
                        r.LogicalResourceId.StartsWith("CloudFormation", StringComparison.InvariantCultureIgnoreCase)
                    );
            }
            catch (AmazonCloudFormationException)
            {
                Error("Could not find IAM stack while looking for CF role, please bootstrap and update");
                return;
            }
            if (resource == null)
            {
                Error("Could not find IAM CF role, looked for one starting with 'CloudFormation'");
                return;
            }
            try
            {
                var identityClient = GetIdentityManagementClient();
                var response = await identityClient.GetRoleAsync(new GetRoleRequest
                {
                    RoleName = resource.PhysicalResourceId
                });
                roleArn = response.Role.Arn;
            }
            catch (AmazonIdentityManagementServiceException)
            {
                Error("Failed to resolve ARN of role '{0}'", resource.PhysicalResourceId);
                return;
            }
            capabilities.Add("CAPABILITY_AUTO_EXPAND");
            template += "Transform: AWS::Serverless-2016-10-31";
        }
        else
        {
            if (!new [] {"iam", "prereqs"}.Contains(BuildEngine.StackType))
            {
                Error("'{0}' is not a recognised stack type", BuildEngine.StackType);
                return;
            }
            stack = $"{BuildEngine.StackName}-{BuildEngine.StackType}";
        }
        Information("Checking for existing stack '{0}'", stack);
        try
        {
            var response = await client.DescribeStacksAsync(new DescribeStacksRequest
            {
                StackName = stack
            });
            if (response.Stacks.Count > 0)
            {
                var s = response.Stacks[0];
                Information("Stack does not need bootstrapping, already exists");
                Information("Created: {0:yyyy-MM-dd} Updated: {1:yyyy-MM-dd} Status: {2}", s.CreationTime, s.LastUpdatedTime, s.StackStatus);
                return;
            }
        }
        catch (AmazonCloudFormationException)
        {
            // great, doesn't exist, proceed
        }
        Information("Creating stack");
        try
        {
            var response = await client.CreateStackAsync(new CreateStackRequest
            {
                StackName = stack,
                RoleARN = roleArn,
                Capabilities = capabilities,
                TemplateBody = template,
                Tags = BuildEngine.StackTags.Select(tag => new Amazon.CloudFormation.Model.Tag
                {
                    Key = tag.Key,
                    Value = tag.Value
                }).ToList()
            });
            Information("Stack created, StackId: {0}", response.StackId);
        }
        catch (AmazonCloudFormationException ex)
        {
            Error(ex.Message);
            return;
        }
    });
