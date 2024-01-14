import concurrent.futures
import gspread
import funtions as funt
import time

creds = gspread.service_account(filename='service_account.json')
sheet = creds.open('PADI-me')

client_sheet = []
acc_sheet = []
sheet_dict = {
    '1': [client_sheet, funt.find_auth],
    '2': [acc_sheet, funt.find_id]
}

qst = input("Client sheets or Source sheets?: [Default = 1] (1/2): ")
use_sheet = sheet_dict[qst][0]
use_func = sheet_dict[qst][1]

with open("/home/vilas/Documents/work_IP/checkproxy-list-dif-auth", "r") as file:
    lines = file.readlines()

stripped_lines = [line.strip() for line in lines]
total_ips = len(stripped_lines)


start = time.perf_counter()


with concurrent.futures.ThreadPoolExecutor() as executor:
    for worksheet in sheet.worksheets():
        if worksheet.title in use_sheet:
            print(f'{funt.Bcolors.HEADER}{worksheet.title}{funt.Bcolors.ENDC}')
            results = executor.map(use_func, stripped_lines, [worksheet] * total_ips)
            funt.result_print(results)


finish = time.perf_counter()
total_time = round(finish - start, 2)
print(f"Finished in {total_time} second{'s'[:int(total_time)^1]}")
