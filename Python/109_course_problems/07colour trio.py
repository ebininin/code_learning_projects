def colour_trio(colours):
    final_colour = colours
    flag = True
    colours_dict = {
        **dict.fromkeys(['ry', 'yr', 'bb'], 'b'),
        **dict.fromkeys(['rb', 'br', 'yy'], 'y'),
        **dict.fromkeys(['by', 'yb', 'rr'], 'r')
    }
    while flag:
        loop_color = ''
        for index, colour in enumerate(final_colour[:-1]):
            colors = colour + final_colour[index + 1]
            loop_color += colours_dict[colors]
        final_colour = loop_color
        if len(final_colour) == 1:
            flag = False
    return final_colour

a = 'rybyr'
print(colour_trio(a))