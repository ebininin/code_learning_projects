import concurrent.futures
import re
import time
import testFunt as Funt

ip_regex = re.compile(r'\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}')

with open("/home/vilas/Documents/work_IP/checkproxy-list-dif-auth", "r") as file:
    lines = file.readlines()

flag = input("Check every port? (Y/N):\n").lower() in ['y', 'yes']
lst = Funt.url_gen_all(lines) if flag else (next(Funt.url_gen(line)) for line in lines)
sorted_lst = sorted(lst, key=Funt.get_port)

start = time.perf_counter()

with concurrent.futures.ProcessPoolExecutor() as executor:
    results = executor.map(Funt.check_proxy, sorted_lst)
    for result in results:
        print(result)

finish = time.perf_counter()
total_time = round(finish - start, 2)
print(f"Finished in {total_time} second{'s'[:int(total_time)^1]}")
