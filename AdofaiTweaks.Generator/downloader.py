from __future__ import print_function
import pickle
import os.path
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request
from pprint import pprint
import xlsxwriter

# If modifying these scopes, delete the file token.pickle.
SCOPES = ['https://www.googleapis.com/auth/spreadsheets.readonly']

# The ID and range of a sample spreadsheet.
SPREADSHEET_ID = '1h5pehiIn1lvYS8s9hSD3Vdp7YtrHzlA-Aa6a-9ky0tQ'


def main():
    """Shows basic usage of the Sheets API.
    Prints values from a sample spreadsheet.
    """
    creds = None
    # The file token.pickle stores the user's access and refresh tokens, and is
    # created automatically when the authorization flow completes for the first
    # time.
    if os.path.exists('token.pickle'):
        with open('token.pickle', 'rb') as token:
            creds = pickle.load(token)
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(
                'credentials.json', SCOPES)
            creds = flow.run_local_server(port=0)
        # Save the credentials for the next run
        with open('token.pickle', 'wb') as token:
            pickle.dump(creds, token)

    service = build('sheets', 'v4', credentials=creds)

    # Call the Sheets API
    sheetsApi = service.spreadsheets()
    spreadsheet = sheetsApi.get(
        spreadsheetId=SPREADSHEET_ID, ranges=[]).execute()

    # Copy sheet data
    workbook = xlsxwriter.Workbook('translations.xlsx')
    for sheet in spreadsheet['sheets']:
        title = sheet['properties']['title']
        worksheet = workbook.add_worksheet(title)
        data = sheetsApi.values().get(spreadsheetId=SPREADSHEET_ID, range=title+'!A1:Z1000').execute()
        for i, row in enumerate(data['values']):
            for j, val in enumerate(row):
                worksheet.write_string(i, j, val)
    workbook.close()


if __name__ == '__main__':
    main()
