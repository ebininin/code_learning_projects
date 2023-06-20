import time

start = time.perf_counter()


def extract_increasing(digits):
    current_largest = 0
    loop_num = ''
    lst = []
    for number in digits:
        loop_num += number
        int_loop = int(loop_num)
        if int_loop > current_largest:
            current_largest = int_loop
            lst.append(loop_num)
            loop_num = ''
            print(lst)


extract_increasing('122333444455555666666')

finish = time.perf_counter()
total_time = round(finish - start, 2)
print(f"Finished in {total_time} second{'s'[:int(total_time)^1]}")