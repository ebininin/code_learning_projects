import re

with open("/home/vilas/Documents/work_IP/checkproxy-list-dif-auth", "r") as file:
    lines = file.readlines()

line_regex = re.compile(r'(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{5}-\d{5})')

with open("/home/vilas/Documents/work_IP/check_results", "w") as file:
    for line in lines:
        ip, port_range = line_regex.search(line).groups()
        first, last = port_range.split('-')
        for port in range(int(first), int(first)+13):
            file.write(f'{ip}:{port}:vilas:Vilas1k\n')
