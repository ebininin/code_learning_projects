def riffle(items, out=True):
    final_list = []
    items_len = len(items)
    items_len_halves = items_len // 2
    for index in range(items_len_halves):
        lst = [items[index], items[index + items_len_halves]]
        final_list.extend(lst)
    if out:
        print(final_list)
    else:
        print(final_list[::-1])


a = [1, 2, 3, 4, 5, 6, 7, 8]
b = []
riffle(a, True)
