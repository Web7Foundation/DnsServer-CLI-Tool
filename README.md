# DnsServer CLI Tool
A command line application to interact with your [DnsServer](https://github.com/Web7Foundation/DnsServer) instance.

## Features
- View zones
- Add zone
- Delete zone
- View records in zone
  
## Setup
### Windows
- Clone the repo
- Open the project in Visual Studio 2022
- Publish the project to default publish folder
- Copy the directory path of the publish folder
- Go to Environment Variables window, click on `Path` under System variables, then below, click `Edit > New` and paste the publish directory into the list

## Usage
- Make sure your [DnsServer](https://github.com/Web7Foundation/DnsServer) instance is running on your local machine on port 5380 (default)
- In your command prompt use the following command with the correct username and password args:
```
dnsserver <username> <password>
```

> Note: Use the 'help' command to list all the commands that are available
