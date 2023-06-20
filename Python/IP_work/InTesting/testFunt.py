import re
import requests

line_regex = re.compile(r'(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})\s+(\d{1,5}-?\d{1,5})?\s+(.+)\s+(.+)')
url_regex = re.compile(r'http://(.*):(.*)@(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,5})$')
port_regex = re.compile(r'\d+$')


def read_line(item):
    ip_address, port_range, user, password = line_regex.search(item).groups()
    return ip_address, port_range, user, password


def read_url(url):
    *rest, address, port = url_regex.search(url).groups()
    return address, port


def get_port(url):
    return int(port_regex.search(url).group())


def url_gen(item):
    ip_address, port_range, user, password = read_line(item)
    try:
        first, last = map(int, port_range.split('-'))
        for port in range(first, last + 1):
            yield f'http://{user}:{password}@{ip_address}:{port}'
    except ValueError:
        yield f'http://{user}:{password}@{ip_address}:{port_range}'


def url_gen_all(items):
    return (url for item in items for url in url_gen(item))


def check_proxy(url):
    proxies = {'http': url}
    address, port = read_url(url)
    try:
        r = requests.get('http://icanhazip.com', proxies=proxies)
        content = r.content.decode().strip()
        return f'IP: {address} - Port: {port}\n{content}\n'
    except Exception:
        return f"{address} - {port}: Failed"
