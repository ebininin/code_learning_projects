def is_cyclops(number):
    digit = 0
    power = 1
    cnt = 0
    while number > power:
        digit += 1
        power *= 10
        cnt += 1
        number = number // 10
        power //= 100
        print(number, digit, cnt, power)
    return cnt % 2 == 1 and number == 0 and digit % 2 == 1


a = 144845061
print(is_cyclops(a))
