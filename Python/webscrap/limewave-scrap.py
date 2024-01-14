import requests
from bs4 import BeautifulSoup


def create_session(url, user, pw, token):
    funt_session = requests.Session()
    login_url = url
    data = {
        "token": token,
        "username": user,
        "password": pw
    }
    print(data["token"])
    try:
        funt_response = funt_session.post(url=login_url, data=data)
        funt_response.raise_for_status()
        if funt_response.status_code == 200:
            print("Login successful!")
            return funt_session
        else:
            print("Login failed.")
            return None
    except requests.exceptions.RequestException as e:
        print("An error occurred:", e)
        return None


# def get_token(url):
#     source = requests.get(url).text
#     login_soup = BeautifulSoup(source, "lxml")
#     token = login_soup.find("input", {"name": "token"}).get("value")
#     return token


url_ = "https://billing.limewave.net/index.php?rp=/login"
username = ""
password = ""

source = requests.get(url_).text
print(source)
# login_soup = BeautifulSoup(source, "lxml")
# token = login_soup.find("input", {"name": "token"}).get("value")
# print(token)
# print(get_token(url_))
# session = create_session(url_, username, password, get_token(url_))
# print(get_token(url_))
# if session:
#     response = session.get("https://billing.limewave.net/clientarea.php?action=services")
#     if response.status_code == 200:
#         print("Authenticated requestsoup successful!")
#         soup = BeautifulSoup(response.content, 'lxml')
#         with open("/home/vilas/Documents/work_IP/check_results", "w") as file:
#             file.write(f"{soup.prettify()}")
#     else:
#         print("Authenticated request failed.")
#
#
#     # tbody = soup.find('tbody')
#     # for tr in soup.find_all('tr'):
#     #     tr_content = tr.text
#     #     print(tr_content)
#     # print(tbody)
#     # trs = tbody.find_all('tr')
#     #
#     # for tr in trs:
#     #     print(tr.text)
