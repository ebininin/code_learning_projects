from bs4 import BeautifulSoup
import requests


login_url = "http://limewave.net/auth/login"
credentials = {"handle": "khang140388@yahoo.com", "password": "Kobiet123"}
session = requests.Session()
response = session.post(login_url, data=credentials)

target_url = 'https://billing.limewave.net/clientarea.php?action=services'

soup = BeautifulSoup(response.content, "html.parser")

tbody = soup.find("tbody")
trs = tbody.find_all('tr')

for tr in trs:
    print(tr.text)
