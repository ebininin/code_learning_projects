# class Tree:
#
#     def __init__(self, value):
#         self.value = value
#         self.left = None
#         self.right = None
#         self.num_occur = 1
#         self.smaller_than_value = 0
#
#
# def insert(tree, value):
#     if not tree:
#         return Tree(value), 0
#     if value < tree.value:
#         tree.smaller_than_value += 1
#


def smaller(arr):
    data, keys, answer = sorted(arr), {x: 0 for x in range(len(arr))}, [0 for _ in range(len(arr))]
    for num in range(len(arr) - 1, -1, -1):
        i, k, c, el = 0, len(arr), 0 , arr[num]
        while i + 1 != k:
            j = (i + k) // 2
            if el >= data[j]:
                i, c = j, c + keys[j]
            else:
                k, keys[j] = j, keys[j] + 1
        answer[num] = c
    return answer


a = [0 for _ in range(len('123123123123123123123123231123123'))]

print(a)