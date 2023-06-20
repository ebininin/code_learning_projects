from collections import deque

a = 'FRLFRRFRFFFFLFLLRRRLLLFF' * 1729

x = 0
y = 0
y_dir = ['north', 'south']
x_dir = ['east', 'west']

direction_lst = ['north', 'east', 'south', 'west']
move_dict = {
    'north': 1,
    'south': -1,
    'west': -1,
    'east': 1
}

rotate_lst = deque(direction_lst)
direction = rotate_lst[0]
for inx, letter in enumerate(a):
    if letter == "R":
        rotate_lst.rotate(-1)
        direction_lst = list(rotate_lst)
    elif letter == "L":
        rotate_lst.rotate(1)
        direction_lst = list(rotate_lst)
    elif letter == "F":
        if direction in x_dir:
            x += move_dict[direction]
        elif direction in y_dir:
            y += move_dict[direction]
    direction = rotate_lst[0]
    print(direction_lst, direction)
    print(x, y)
print(x, y)

