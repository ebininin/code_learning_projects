# Import the libraries
import requests
from bs4 import BeautifulSoup
import pandas as pd
import schedule
import time


# Define a function to scrape the data
def scrape_data():
    # Make a request to the login page
    login_url = "https://my.contabo.com/login"
    login_response = requests.get(login_url)

    # Parse the HTML content and get the CSRF token
    login_soup = BeautifulSoup(login_response.content, "html.parser")
    csrf_token = login_soup.find("input", {"name": "_csrf_token"})["value"]

    # Prepare the login data with your username and password
    login_data = {
        "_csrf_token": csrf_token,
        "username": "khang140388@yahoo.com",
        "password": "Kobiet123"
    }

    # Create a session object to persist the cookies
    session = requests.Session()

    # Post the login data and get the response
    post_response = session.post(login_url, data=login_data)

    # Check if the login was successful
    if post_response.url == "https://my.contabo.com/":
        print("Login successful")

        # Make a request to the cloud instances page
        cloud_url = "https://my.contabo.com/cloud-instances"
        cloud_response = session.get(cloud_url)

        # Parse the HTML content and find the table rows
        cloud_soup = BeautifulSoup(cloud_response.content, "html.parser")
        table_rows = cloud_soup.find_all("tr", {"class": "instance"})

        # Create an empty list to store the data
        data_list = []

        # Loop through each table row and extract the data
        for row in table_rows:
            # Find the IP address, InstanceID, and expired date elements
            ip_address = row.find("td", {"class": "ip-address"}).text.strip()
            instance_id = row.find("td", {"class": "instance-id"}).text.strip()
            expired_date = row.find("td", {"class": "expired-date"}).text.strip()

            # Create a dictionary with the data
            data_dict = {
                "IP Address": ip_address,
                "InstanceID": instance_id,
                "Expired Date": expired_date
            }

            # Append the dictionary to the list
            data_list.append(data_dict)

        # Create a pandas dataframe from the list
        df = pd.DataFrame(data_list)

        # Save the dataframe to a spreadsheet file
        df.to_excel("contabo_data.xlsx", index=False)

        print("Data saved to contabo_data.xlsx")

    else:
        print("Login failed")

scrape_data()
# Schedule the function to run weekly on Monday at 10:00 AM
# schedule.every().monday.at("10:00").do(scrape_data)
#
# # Run the scheduler in a loop
# while True:
#     # Run pending tasks
#     schedule.run_pending()
#     # Sleep for one second
#     time.sleep(1)
