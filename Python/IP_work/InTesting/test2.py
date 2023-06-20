import requests
import concurrent.futures
import re

line_regex = re.compile(r'(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})\t+(\d{1,5})-?(\d{1,5})?\t+(.*)\t+(.*)')
url_regex = re.compile(r'http://(.*):(.*)@(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,5})$')
port_regex = re.compile(r'\d+$')


def read_line(item, group_num):
    line_result = line_regex.search(item).group(group_num)
    return line_result


def read_url(url, no_group):
    re_sult = url_regex.search(url).group(no_group)
    return re_sult


def get_port(url):
    return int(port_regex.search(url).group())


def url_gen_lst(item, out=False):
    usr = read_line(item, 4)
    pw = read_line(item, 5)
    ip = read_line(item, 1)
    first = int(read_line(item, 2))
    last = int(read_line(item, 3)) + 1
    rng = range(first, last)
    if not out:
        return [f'http://{usr}:{pw}@{ip}:{first}']
    else:
        return list(f'http://{usr}:{pw}@{ip}:{port}' for port in rng)


def check_proxy(url):
    proxies = {'http': url}
    host = read_url(url, 3)
    port = read_url(url, 4)
    try:
        r = requests.get('http://icanhazip.com', proxies=proxies)
        content = r.content.decode().strip()
        return f'IP: {host} - Port: {port}\n{content}\n'
    except Exception:
        return f"{host} - {port}: Failed"


def url_gen(items, out):
    for line in items:
        yield from url_gen_lst(line, out=out)


# with open(input("Enter file path:\n"), "r") as file:
with open("/home/vilas/Documents/work_IP/checkproxy-list-dif-auth", "r") as file:
    lines = file.readlines()

flag = False
qst = input("Check every port? (Y/N):\n").lower()
if qst in ['y', 'yes']:
    flag = True
lst = url_gen(lines, flag)

sorted_lst = sorted(lst, key=get_port)

# with concurrent.futures.ProcessPoolExecutor() as executor:
#     results = executor.map(check_proxy, sorted_lst)
#     for result in results:
#         print(result)

print(read_line(lines[0], 3))