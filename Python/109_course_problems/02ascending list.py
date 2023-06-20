def is_ascending(items=None):
    if not items:
        return True
    last = items[0]
    for num in items[1:]:
        if num <= last:
            return False
        last = num
    return True


a = [4, 5, 6, 7, 3, 7, 9]
b = [-5, 10, 99, 123456]
c = []
d = [2, 3, 3, 4, 5]
f = [0]
print(is_ascending(b))
