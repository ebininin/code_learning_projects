def give_change(amount: int, coins: list):
    result = []
    for coin in coins:
        rep_time = amount // coin
        for _ in range(rep_time):
            result.append(coin)
            amount -= coin
    return result


print(give_change(64, [50, 25, 10, 5, 1]))
print(give_change(123, [100, 25, 10, 5, 1]))
