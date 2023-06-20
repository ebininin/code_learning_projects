import re

with open('/home/vilas/Documents/VPS proxy/pỵcommand/contabo-filter', 'r') as file:
    fi = file.readlines()

re_ip = re.compile(r'\d{9}')

for line in fi:
    ip = re.findall(re_ip, line)
    for i in ip:
        # print(f'./cntb get instance {i}')
        # print(f'./cntb stop instance {i}')
        # print(f'./cntb start instance {i}')
        print(f'./cntb restart instance {i}')
        # print(f'./cntb cancel instance {i}')
        # print(f'./cntb reinstall instance {i} --imageId "8dc65545-3ada-4467-bfb6-107b9b832aaf" --rootPassword 55074 --defaultUser "root"') # khang
        # print(f'./cntb reinstall instance {i} --imageId "8dc65545-3ada-4467-bfb6-107b9b832aaf" --rootPassword 64051 --defaultUser "root"') #crazy
        # print(f'./cntb reinstall instance {i} --imageId "8dc65545-3ada-4467-bfb6-107b9b832aaf" --rootPassword 64053 --defaultUser "root"')   #crazy2
        # print(f'./cntb reinstall instance {i} --imageId "8dc65545-3ada-4467-bfb6-107b9b832aaf" --rootPassword 67416 --defaultUser "root"')   #crazy3
        # print(f'./cntb reinstall instance {i} --imageId "8dc65545-3ada-4467-bfb6-107b9b832aaf" --rootPassword 64055 --defaultUser "root"')  # hangdang
