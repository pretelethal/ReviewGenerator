
# Review Generator
generates random reviews based from training data

## Requirements

.net 6.0
node.js
npm

## npm

```
npm install
```
if required

## Visual Studio 2022

recommended to launch application via Visual Studio or other compatible IDE for debugging

```
dotnet dev-certs https --trust
```
**may need to be run if VS2022 requires it for this project

### appsettings.json

```DataFilePath```
location of the file holding the training data
**will need to be set prior to application start due to need to train data
ex. `/app/files/data.json` or `..\\files\\data.json`

```MarkovKeySize```
the key size used during the markov chain training

```CharLimit```
the target character limit for the randomized text output
```
{
  "Config": {
    "DataFilePath": "/path/to/data/file.json",
    "MarkovKeySize": 5,
    "CharLimit": 100
  }
}
```

### launchSettings.json

```ASPNETCORE_ENVIRONMENT```
needs to target the corresponding appsettings.json file targeting the env of the same name
ex. `Docker` for docker production or `Development` for local development

```DockerfileRunArguments```
needs to be altered to target the location of training data we want to mount to the docker container
```
"Docker": {
	"commandName": "Docker",
	"launchBrowser": true,
	"launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}",
	"environmentVariables": {
	  "ASPNETCORE_ENVIRONMENT": "Docker",
	  "ASPNETCORE_URLS": "http://+:80"
	},
	"publishAllPorts": false,
	"useSSL": false,
	"DockerfileRunArguments": "--mount type=bind,source=\"C:\\host\\file\\path\",target=\"/target/mount/path\""
  }
}
```

## dotnet

test the project	
```
dotnet test
```

publish the project
```
dotnet publish -c Release
```	

run the project (if desired, otherwise proceed to docker steps)
```
dotnet run --project=./ReviewGenerator
```

## Docker
	
`/app` is the default root for this project in docker
if mounting host file path make sure to use `/app/path/to/file/dir`
	
build the image
```
docker build -t {image-name} -f ReviewGenerator\Dockerfile .
```

generate and run the container
```
docker run -d -p {desired_host_port}:80 --env ASPNETCORE_ENVIRONMENT="Docker" --mount type=bind,source=c:\host\file\path\,target=/target/mount/path --name {container-name} {image-name}
```
	
### Notes

dockerfile in this project configured for linux target. ran from windows using WSL 2.0.
	
https disabled for this project's docker image/container. special certificate rules have yet to be put in place to allow proper usage.

if running docker container from visual studio. make sure to run it in 'Release' profile, SPA proxy for debugging docker container is not yet configured in this project.
		
## Data Source(s)

https://cseweb.ucsd.edu/~jmcauley/datasets/amazon/links.html
