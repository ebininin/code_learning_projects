import re

with open('/home/vilas/Documents/work_IP/ip-filter', 'r') as file:
    fi = file.readlines()

with open('/home/vilas/Documents/work_IP/ip-current', 'r') as file2:
    current_ip = [each_line.strip() for each_line in file2]

re_ip = re.compile(r'IP: \d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}')
re_ipp = re.compile(r'\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}')


for line in fi:
    ip = re.findall(re_ip, line)
    for i in ip:
        ipp = re.findall(re_ipp, line)
        for pp in ipp:
            print(pp)
