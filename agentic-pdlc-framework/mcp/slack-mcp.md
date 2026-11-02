# Slack MCP Configuration

## Package
`@modelcontextprotocol/server-slack`

## Setup

### 1. Create Slack App
1. Go to https://api.slack.com/apps → Create New App → From Scratch
2. Name it "PDLC Notifier", select your workspace
3. Go to "OAuth & Permissions" → Bot Token Scopes, add:
   - `chat:write` — post messages
   - `channels:read` — list channels
   - `users:read` — resolve user names
4. Install App to Workspace → copy "Bot User OAuth Token"
5. Invite the bot to relevant channels: `/invite @PDLC Notifier`

### 2. Get Team ID
Found in: Workspace settings URL → `T0XXXXXXXX`
Or: https://api.slack.com/methods/auth.test → `team_id`

### 3. Set Environment Variables
```bash
export SLACK_BOT_TOKEN="xoxb-xxxxxxxxxxxx"
export SLACK_TEAM_ID="T0XXXXXXXXX"
```

## Available MCP Tools

```
mcp__slack__list_channels
  → Returns available channels

mcp__slack__post_message(channel_id, text)
  → Post plain text message

mcp__slack__post_message(channel_id, blocks)
  → Post rich message with Block Kit layout

mcp__slack__reply_to_thread(channel_id, thread_ts, text)
  → Reply to an existing message thread

mcp__slack__get_channel_history(channel_id, limit)
  → Read recent messages
```

## Message Templates

### Deployment Notification
```json
{
  "blocks": [
    {
      "type": "header",
      "text": {"type": "plain_text", "text": "🚀 Release v1.2.3 deployed to Production"}
    },
    {
      "type": "section",
      "fields": [
        {"type": "mrkdwn", "text": "*Project:*\n[project name]"},
        {"type": "mrkdwn", "text": "*Environment:*\nProduction"},
        {"type": "mrkdwn", "text": "*Deployed by:*\nPDLC Agent"},
        {"type": "mrkdwn", "text": "*Time:*\n[timestamp]"}
      ]
    },
    {
      "type": "section",
      "text": {"type": "mrkdwn", "text": "*What's in this release:*\n• Feature: [description]\n• Fix: [description]"}
    },
    {
      "type": "actions",
      "elements": [
        {"type": "button", "text": {"type": "plain_text", "text": "Release Notes"}, "url": "[confluence URL]"},
        {"type": "button", "text": {"type": "plain_text", "text": "PR"}, "url": "[github PR URL]"}
      ]
    }
  ]
}
```

### Hotfix Alert
```json
{
  "blocks": [
    {
      "type": "header",
      "text": {"type": "plain_text", "text": "🚨 P0 Hotfix deployed"}
    },
    {
      "type": "section",
      "text": {"type": "mrkdwn", "text": "*[TICKET-ID]*: [one-line description]\nFixed and deployed to production."}
    }
  ]
}
```

### Weekly Health Summary
```
🏥 *Weekly Health Report — [Project]*
✅ Tests: 142 passing | Coverage: 87%
⚠️  Security: 1 high CVE (express@4.18.2) — patch scheduled
📊 Open PRs: 3 | Stale: 1
Full report: [Confluence link]
```

## Updating project.yaml

```yaml
integrations:
  notifications:
    onDeployment: true
    channel: "#deployments"     # Slack channel name (without #) or ID
    alertsChannel: "#alerts"    # For maintenance alerts and health issues
```

## Channel ID vs Name

MCP tools require channel ID (e.g., `C0XXXXXXXXX`), not the channel name. Get IDs:
```bash
curl -H "Authorization: Bearer $SLACK_BOT_TOKEN" \
  https://slack.com/api/conversations.list | jq '.channels[] | {name, id}'
```
