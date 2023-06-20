import names
import time
import concurrent.futures


def people(nums):
    for i in range(nums):
        person = {
            "id": i,
            "name": names.get_full_name(),
            "ran": names.get_first_name()
        }
        yield person


start = time.time()
with concurrent.futures.ProcessPoolExecutor() as executor:
    civi = executor.map(people, '10000000')
    for ci in civi:
        print(next(ci))

end = time.time()
print(end-start)
