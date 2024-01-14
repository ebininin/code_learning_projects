import requests
from bs4 import BeautifulSoup
from concurrent.futures import ThreadPoolExecutor

login_url = "https://billing.limewave.net/index.php?rp=/login"
login_page_url = "https://billing.limewave.net/index.php?rp=/login"
services_url = "https://billing.limewave.net/clientarea.php?action=services"
product_details_url_template = "https://billing.limewave.net/clientarea.php?action=productdetails&id={id}"

# Fetch the login page to retrieve the token
session = requests.Session()
login_page_response = session.get(login_page_url)
login_page_soup = BeautifulSoup(login_page_response.content, "html.parser")
token = login_page_soup.find("input", {"name": "token"}).get("value")

# Prepare the login payload with the retrieved token
payload = {
    "token": token,
    "username": "",
    "password": ""
}

# Send the login request
response = session.post(login_url, data=payload)

# Check if login was successful
if response.status_code == 200:
    print("Login successful")

    # Fetch the services page to find the IDs of VPS instances
    services_response = session.get(services_url)
    services_soup = BeautifulSoup(services_response.content, "html.parser")
    td_tags = services_soup.find_all("td", attrs={"data-domain": True, "data-element-id": True})
    # with open("/home/vilas/Documents/work_IP/check_results", "w") as file:
    #     file.write(f"{services_soup}")
    # print(services_soup)
    list_ids = []
    if td_tags:
        for td in td_tags:
            data_element_id = td["data-element-id"]
            list_ids.append(data_element_id)
            print("idvps: ", data_element_id)
    else:
        print("No matching <td> elements found.")

    print("length id: ", len(list_ids))

    # Create a list to store the data for each VPS instance
    vps_data = []

    # Define a function to fetch data for a given VPS ID
    def fetch_vps_data(id):
        product_details_url = product_details_url_template.format(id=id)
        product_details_response = session.get(product_details_url)
        soup = BeautifulSoup(product_details_response.content, "html.parser")

        div_elements = soup.select('div.col-sm-7.text-left')
        hostname = div_elements[0].get_text(strip=True)
        ip = div_elements[1].get_text(strip=True)

        next_due_date_element = soup.find("h4", string="Next Due Date")
        if next_due_date_element:
            next_due_date = next_due_date_element.find_next_sibling(text=True).strip()
        else:
            next_due_date = "Next Due Date not found"

        # Print the data for each VPS instance
        print("Hostname:", hostname)
        print("IP:", ip)
        print("Next Due Date:", next_due_date)
        print()

        # Append the data to the vps_data list
        vps_data.append((hostname, ip, next_due_date))

    # Use multithreading to fetch data for multiple VPS instances concurrently
    with ThreadPoolExecutor(max_workers=10) as executor:
        executor.map(fetch_vps_data, list_ids)

    # Export the data to a text file
    with open("vps_data.txt", "w") as file:
        file.write("Hostname\t\tIP\t\t\t\tNext Due Date\n")
        for data in vps_data:
            file.write(f"{data[0]}\t\t{data[1]}\t\t{data[2]}\n")
            print(data)
    print("Data exported to vps_data.txt file.")

else:
    print("Login request failed with status code:", response.status_code)
