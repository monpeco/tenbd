#include <stdio.h>

void main(void)
{
    const int i = 2;
    int *p;
    p = &i;         //Produces a compilatin error with c++ compiler (not with c compiler)
    *p = 3;
    printf("i = %d\n", i);
}