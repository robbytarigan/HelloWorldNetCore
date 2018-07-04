# Sample VS Code launch.json

```json
{
    // Use IntelliSense to find out which attributes exist for C# debugging
    // Use hover for the description of the existing attributes
    // For further information visit https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            // If you have changed target frameworks, make sure to update the program path.
            "program": "${workspaceFolder}/sample/bin/container/Debug/netcoreapp2.1/sample.dll",
            "args": [],
            "cwd": "${workspaceFolder}/sample",
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart",
            "launchBrowser": {
                "enabled": true,
                "args": "${auto-detect-url}",
                "windows": {
                    "command": "cmd.exe",
                    "args": "/C start ${auto-detect-url}"
                },
                "osx": {
                    "command": "open"
                },
                "linux": {
                    "command": "xdg-open"
                }
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Remote Attach",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickRemoteProcess}",
            "pipeTransport": {
                "debuggerPath": "~/vsdbg/vsdbg",
                "pipeCwd": "${workspaceFolder}",
                "windows": {
                    "pipeProgram": "plink.exe",
                    "pipeArgs": [
                        "-l",
                        "root",
                        "-pw",
                        "Docker!",
                        "localhost",
                        "-P",
                        "2222",
                        "-T"
                    ],
                    "quoteArgs": true
                },
                "osx": {
                    "pipeProgram": "ssh",
                    "pipeArgs": [
                        "root@localhost:2222"
                    ],
                    "quoteArgs": true
                },
                "linux": {
                    "pipeProgram": "ssh",
                    "pipeArgs": [
                        "root@localhost:2222"
                    ],
                    "quoteArgs": true
                }
            },
            "sourceFileMap": {
                "/app/sample": "${workspaceFolder}/sample",
            },
            "justMyCode": true
        }
    ]
}
```

# How to call MIEngine from Visual Studio
From Command Window, type:
```
DebugAdapterHost.Launch /LaunchJson:"E:\dockerdata\dotnetexplore\app\HelloWorldNetCore\2.1\react\MIEngineConfig\MIEngineAttach.json"
```

# How to run docker
First build image using build/Debug.Dockerfile:
```
docker build -f Debug.Dockerfile -t reactbuild:debug .
```

Run:
```
docker run --rm -it -p 8000:80 -p 8001:443 -p 2222:2222 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_ENVIRONMENT=Development -v $env:appdata\microsoft\UserSecrets\:/root/.microsoft/usersecrets -v $env:userprofile\.aspnet\https:/root/.aspnet/https/ -v e:/dockerdata/dotnetexplore/app/HelloWorldNetCore/2.1/react/:/app -w /app/sample reactbuild:debug
```