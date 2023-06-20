import subprocess # This will import the subprocess module
output = "Hello, PowerShell!" # This will assign a string to a variable
command = f"echo {output}" # This will create a PowerShell command to echo the output
result = subprocess.run(["powershell", "-Command", command], capture_output=True) # This will run the command using PowerShell and capture its output
print(result.stdout.decode())