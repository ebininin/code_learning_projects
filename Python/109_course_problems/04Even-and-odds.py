def only_odd_digits(n):
    for num in str(n):
        if int(num) % 2 == 0:
            return False
    return True


a = 71358
print(only_odd_digits(a))