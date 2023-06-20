def count_dominators(items):
    count = 0
    for index, number in enumerate(items):
        if any(num >= number for num in items[index + 1:]):
            pass
        else:
            count += 1
    return count


a = [-2, 5, -1, -3, -5]

print(count_dominators(a))
