this is the code from
https://aws.amazon.com/blogs/compute/developing-net-core-aws-lambda-functions

Preconditions:
AWS CLI 
https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html

AWS SAM CLI 
https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html

Amazon.Lambda.Tools
dotnet tool install --global Amazon.Lambda.Tools
https://stackoverflow.com/questions/48472193/deploying-to-aws-lambda-from-visual-studio-team-services-dotnet-lamba-not-found

Docker is running

XRay is running in docker
https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html

Changes from article:
Use the follwoing command to generate request:
sam local generate-event apigateway aws-proxy > testApiRequest.json

Also replace tabs with spaces in template.yaml



