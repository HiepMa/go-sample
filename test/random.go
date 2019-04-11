package main

import (
    "fmt"
    "math/rand"
    "time"
)

func rd() {
    rand.Seed(time.Now().UnixNano())
    fmt.Println(rand.Intn(30))
    fmt.Println(rand.Intn(30))
    fmt.Println(rand.Intn(30))
    fmt.Println(rand.Intn(30))
    fmt.Println(rand.Intn(30))
}