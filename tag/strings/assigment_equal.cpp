#include<iostream>
#include<cstdio>
using namespace std;


int main()
{
    string pRutax="";
    char valorPar[80] = "This is string assignment operator";
    string pRutaArch="Another string";

    pRutax=valorPar;
    cout << "pRutax: ["<< pRutax << "]" << endl;
    
    pRutax=pRutaArch;
    cout << "pRutax: ["<< pRutax << "]" << endl;    
    
    return 0;
}