from fastapi import Request, FastAPI
import pandas as pd
import csv
import json
import glob
import time

app = FastAPI()

filename = 'data/results.csv'
file_present = glob.glob(filename)

if not file_present:
    df = pd.DataFrame(columns = ["timestamp", "participant", "feedbackType", "test_nr", "Time", "Distance", "target", "radius", "Path"])
    df.to_csv(filename, index=False)

@app.get("/")
async def root():
    return {"message": "Hello World"}

@app.post("/results/")
async def send_results(request: Request):
    data = await request.json()
    for i in data['results']:
        row = { "timestamp": [time.time()], "participant": [data['participant']], "feedbackType": [data['feedbackType']], "test_nr": [i['ID']], "Time": [i['Time']], "Distance": [i['Distance']], "target": [i['target']], "radius": [data['radius']], "Path": [i['Path']] }
        df = pd.DataFrame(row, columns = ["timestamp", "participant", "feedbackType", "test_nr", "Time", "Distance", "target", "radius", "Path"])
        df.to_csv('data/results.csv', mode='a', header=False, index=False)
    return {"message": "Results saved"}
