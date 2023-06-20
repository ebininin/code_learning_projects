import requests
import concurrent.futures
import re
import time

start = time.perf_counter()

port_regex = re.compile(r'@(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,5})$')

# with open(input("Enter file path:\n"), "r") as file:
with open("/home/vilas/Documents/work_IP/checkproxy-list", "r") as file:
    lines = file.readlines()

user = lines[0][5:].strip()
password = lines[1][9:].strip()
first_port = lines[2][11:].strip()
last_port = lines[3][10:].strip()
ports = range(int(first_port), int(last_port) + 1)
ips = list(ip.strip() for ip in lines[6:])
ip_urls = list(f'http://{user}:{password}@{ip}:' for ip in ips)
completed_url = list(f'{ip_url}{port}' for port in ports for ip_url in ip_urls)
# completed_url = list(f'{ip_url}{port}' for ip_url in ip_urls for port in ports)


def check_proxy(url):
    proxies = {'http': url}
    host = port_regex.search(url).group(1)
    port = port_regex.search(url).group(2)
    try:
        r = requests.get('http://icanhazip.com', proxies=proxies)
        content = r.content.decode().strip()
        return f'IP: {host} - Port: {port}\n{content}\n'
    except Exception:
        return f'{host} - {port}: Failed'


with concurrent.futures.ProcessPoolExecutor() as executor:
    results = executor.map(check_proxy, completed_url)

    for result in results:
        print(result)


finish = time.perf_counter()
total_time = round(finish - start, 2)
print(f"Finished in {total_time} second{'s'[:int(total_time)^1]}")
