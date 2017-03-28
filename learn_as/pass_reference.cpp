#include <stdio.h>
#include <stdlib.h>

int i = 1, j = 2;
void permutar(int &a, int &b);

int main(void)
{
    printf("\ni = %d, j = %d", i, j);
    permutar(i, j);
    printf("\ni = %d, j = %d\n", i, j);
}

void permutar(int &a, int &b)
{
    int temp;
    temp = a;
    a = b;
    b = temp;
}