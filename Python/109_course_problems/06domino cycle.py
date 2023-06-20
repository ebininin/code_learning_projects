def domino_cycle(tiles=None):
    if not tiles:
        return True
    first_num = tiles[0][0]
    latest_num = tiles[0][-1]
    for domino in tiles[1:]:
        if domino[0] == latest_num:
            latest_num = domino[-1]
        else:
            return False
        if domino == tiles[-1]:
            return domino[-1] == first_num


a = [(5, 2), (2, 3), (4, 5)]
print(domino_cycle(a))
