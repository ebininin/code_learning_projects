import pexpect
from getpass import getpass, getuser

user = getuser()
output = pexpect.spawn("sudo apt-get upgrade")
output.expect_exact(f"[sudo] password for {user}: ")
output.sendline(getpass(f'Password of "{user}": '))
output.expect(pexpect.EOF, timeout=10)
print(output)