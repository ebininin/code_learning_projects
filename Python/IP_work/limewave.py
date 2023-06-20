import requests


def create_session(username, password):
    session = requests.Session()

    login_url = "https://billing.limewave.net/index.php?rp=/login"  # Replace with the actual login URL
    data = {
        "username": username,
        "password": password
    }

    try:
        response = session.post(url=login_url, data=data)
        response.raise_for_status()  # Raise an exception for any HTTP errors

        # Check if login was successful based on response data or status code
        if response.status_code == 200:
            print("Login successful!")
            # Do something with the logged-in session
            return session
        else:
            print("Login failed.")
            return None
    except requests.exceptions.RequestException as e:
        print("An error occurred:", e)
        return None

# Usage
username = "khang140388@yahoo.com"
password = "Kobiet123"

session = create_session(username, password)
if session:
    # Use the session to make authenticated requests
    response = session.get("https://billing.limewave.net/clientarea.php?action=services")  # Replace with a protected resource URL
    if response.status_code == 200:
        # Process the response
        print("Authenticated request successful!")
    else:
        print("Authenticated request failed.")
