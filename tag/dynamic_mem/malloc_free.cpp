#include <iostream>
#include <string.h>
#include <stdlib.h>
using namespace std;

int main()
{
    double* myPtr = (double*)malloc(sizeof(double)*5);
    free(myPtr);
    myPtr = NULL;       //without this, if we try to free myPtr will produce an execution error

    {
        int length = 4;
    
        int *sieve2 = (int *)malloc(sizeof(int)*length);
        int *sieve3 = (int *)malloc( sizeof *sieve3 * length );
    }
    return 0;
}
