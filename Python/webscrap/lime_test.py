import requests
from bs4 import BeautifulSoup


# def get_token(url_):
#     source = requests.get(url_).text
#     login_soup = BeautifulSoup(source, "lxml")
#     tk = login_soup.find("input", {"name": "token"}).get("value")
#     return tk


def create_session(url_, usr, pw):
    funt_session = requests.Session()
    source = funt_session.get(url_)
    login_soup = BeautifulSoup(source.content, "lxml")
    tk = login_soup.find("input", {"name": "token"}).get("value")

    data = {
        "token": tk,
        "username": usr,
        "password": pw
    }

    try:
        funt_response = funt_session.post(url_, data=data)
        funt_response.raise_for_status()  # Raise an exception for any HTTP errors

        # Check if login was successful based on response data or status code
        if funt_response.status_code == 200:
            print("Login successful!")
            # Do something with the logged-in session
            return funt_session
        else:
            print("Login failed.")
            return None
    except requests.exceptions.RequestException as e:
        print("An error occurred:", e)
        return None


url = "https://billing.limewave.net/index.php?rp=/login"
username = ""
password = ""

session = create_session(url, username, password)
if session:
    # Use the session to make authenticated requests
    response = session.get("https://billing.limewave.net/clientarea.php?action=services")  # Replace with a protected resource URL
    if response.status_code == 200:
        # Process the response
        print("Authenticated request successful!")
        soup = BeautifulSoup(response.content, 'lxml')
        for tr in soup.find_all('tr'):
            tr_content = tr.text
            print(tr_content)
    else:
        print("Authenticated request failed.")
